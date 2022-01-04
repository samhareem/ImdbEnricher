using System;
using System.Collections.Generic;
using System.IO;
using IMDBEnricher.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Text;

namespace IMDBEnricher.Readers
{
    public class ImdbDataSetUpdater : IImdbDataSetUpdater
    {
        private readonly ILogger<ImdbDataSetUpdater> _logger;

        private string _datasetDirectory;
        private string _imdbTitleFilePath;
        private IDictionary<string, IReadOnlyCollection<CrewMember>> _titleDirectors = 
            new Dictionary<string, IReadOnlyCollection<CrewMember>>();
        private IDictionary<string, (float? AverageRating, int NumberOfRatings)> _titleRatings = 
            new Dictionary<string, (float? AverageRating, int NumberOfRatings)>();
        //TODO Would like it if this could be a ReadOnlyCollection as well. Try and figure something out without hurting performance.
        private IDictionary<string, IList<TitleName>> _titleNames = new Dictionary<string, IList<TitleName>>();
        
        //IMDB datasets are presented in a tab delimited format
        private const char Delimiter = '\t';

        public ImdbDataSetUpdater(ILogger<ImdbDataSetUpdater> logger, IConfiguration config)
        {
            _logger = logger;
            _datasetDirectory = Path.GetFullPath(config.GetValue<string>("datasetDirectory"));
            _imdbTitleFilePath = Path.GetFullPath(config.GetValue<string>("imdbTitleFilePath"));
        }

        public void UpdateTitleInformation(string? datasetDirectory, string? dataFilePath)
        {
            //Override config defaults if parameters provided
            _datasetDirectory = datasetDirectory ?? _datasetDirectory;
            _imdbTitleFilePath = dataFilePath ?? _imdbTitleFilePath;
            
            //Build dataset paths based on dataset directory
            var crewDatasetPath = Path.Combine(_datasetDirectory, "name.basics.tsv/data.tsv");
            var titleCrewDatasetPath = Path.Combine(_datasetDirectory, "title.crew.tsv/data.tsv");
            var ratingsDatasetPath = Path.Combine(_datasetDirectory, "title.ratings.tsv/data.tsv");
            //var titleNameDatasetPath = Path.Combine(_datasetDirectory, "title.akas.tsv/data.tsv");
            var titleDatasetPath = Path.Combine(_datasetDirectory, "title.basics.tsv/data.tsv");
            
            _titleDirectors = new Dictionary<string, IReadOnlyCollection<CrewMember>>();
            _titleRatings = new Dictionary<string, (float? AverageRating, int NumberOfRatings)>();
            _titleNames = new Dictionary<string, IList<TitleName>>();
            
            ParseCrew(crewDatasetPath, titleCrewDatasetPath);
            ParseRatings(ratingsDatasetPath);
            //ParseTitleNames(titleNameDatasetPath);
            ParseAndWriteTitles(titleDatasetPath);
        }
        
        public void ParseCrew(string crewDatasetPath, string titleCrewDatasetPath)
        {
            _logger.LogInformation( $"Parsing cast and crew from {Path.GetFileName(crewDatasetPath)}");

            using var crewReader = new StreamReader(Path.GetFullPath(crewDatasetPath));

            //Skip header row
            crewReader.ReadLine();

            var crew = new Dictionary<string, CrewMember>();
            //Loop through the file, parsing each crew member and printing out any errors
            while (!crewReader.EndOfStream)
            {
                //Parse crew information
                var crewInfo = crewReader.ReadLine().Split(Delimiter);
                var crewId = crewInfo[0];
                var primaryName = crewInfo[1];
                
                //Create object containing basic crew member information
                crew.Add(crewId, new CrewMember(crewId, primaryName));    
            }

            using var titleCrewReader = new StreamReader(Path.GetFullPath(titleCrewDatasetPath));
            
            //Loop through the file, parsing each title and printing out any error
            while (!titleCrewReader.EndOfStream)
            {
                var titleCrewInfo = titleCrewReader.ReadLine().Split(Delimiter);
                
                //Parse title director information
                var titleId = titleCrewInfo[0];
                var directorIds = titleCrewInfo[1].Split(',');

                var directors = new List<CrewMember>();
                foreach (var directorId in directorIds)
                {
                    if (crew.TryGetValue(directorId, out var director))
                    {
                        directors.Add(director);
                    }
                }
                
                //Create object containing basic information, director information and rating information
                _titleDirectors.Add(titleId, directors);    
            }
        }
        
        public void ParseRatings(string datasetPath)
        {
            _logger.LogInformation( $"Parsing title ratings from {Path.GetFileName(datasetPath)}");

            using var ratingsReader = new StreamReader(Path.GetFullPath(datasetPath));
            
            //Skip header row
            ratingsReader.ReadLine();
            
            //Loop through the file, parsing each title rating and printing out any error
            while (!ratingsReader.EndOfStream)
            {
                //Parse ratings information
                var titleRatingsInfo = ratingsReader.ReadLine().Split(Delimiter);
                var titleId = titleRatingsInfo[0] ?? "";
                //TODO Replace string replace with culture information
                float.TryParse(titleRatingsInfo[1].Replace('.', ','), out var parsedAverageRating);
                int.TryParse(titleRatingsInfo[2], out var parsedTotalRatings);
                
                //Add title ratings to dictionary
                _titleRatings.Add(titleId, (parsedAverageRating, parsedTotalRatings));   
            }
        }
        
        public void ParseTitleNames(string datasetPath)
        {
            _logger.LogInformation( $"Parsing title names from {Path.GetFileName(datasetPath)}");

            using var namesReader = new StreamReader(Path.GetFullPath(datasetPath));
            
            //Skip header row
            namesReader.ReadLine();
            
            var result = new Dictionary<string, IList<TitleName>>();
            //Loop through the file, parsing each title rating and printing out any error
            while (!namesReader.EndOfStream)
            {
                //Parse name information
                var titleNameInfo = namesReader.ReadLine().Split(Delimiter);
                var titleId = titleNameInfo[0] ?? "";
                var name = titleNameInfo[2] ?? "";

                var titleName = new TitleName(titleId, name);
                //Add or append title name to dictionary
                if (result.ContainsKey(titleId))
                {
                    result[titleId].Add(titleName);
                }
                else
                {
                    result[titleId] = new List<TitleName> { titleName };
                }
            }

            _titleNames = result;
        }
        
        private void ParseAndWriteTitles(string datasetPath)
        {
            _logger.LogInformation( $"Parsing titles from {Path.GetFileName(datasetPath)}");
            
            using var reader = new StreamReader(Path.GetFullPath(datasetPath));
            using var writer = new StreamWriter(Path.GetFullPath(_imdbTitleFilePath));
            
            //Configure CSV-serializer to not use header
            CsvConfig<Title>.OmitHeaders = true;
            
            //Skip header row
            reader.ReadLine();

            //Loop through the file, parsing each title and printing out any error
            while (!reader.EndOfStream)
            {
                var titleInfo = reader.ReadLine().Split(Delimiter);
                //For now, we want to limit the results to actual titles and disregard episodes of tv series
                //TODO Add config for which types to allow
                var titleType = titleInfo[1];
                if (titleType.Equals("tvEpisode"))
                {
                    continue;
                }
                    
                //Parse basic title information    
                var titleId = titleInfo[0];
                var primaryTitle = titleInfo[2];
                var originalTitle = titleInfo[3];
                var startYear = int.TryParse(titleInfo[5], out var parsedStartYear) ? (int?) parsedStartYear : null;
                var endYear = int.TryParse(titleInfo[6], out var parsedEndYear)  ? (int?) parsedEndYear: null;
                var runtime = int.TryParse(titleInfo[7], out var parsedRuntime)  ? (int?) parsedRuntime: null;
                var genres = titleInfo[8];
                
                //Try and add director and ratings information from datasets parsed earlier
                var directors = _titleDirectors.TryGetValue(titleId, out var fetchedDirectors) ? 
                    fetchedDirectors : Array.Empty<CrewMember>();
                var ratings = _titleRatings.TryGetValue(titleId, out var fetchedRatings)
                    ? fetchedRatings : (AverageRating: null, NumberOfRatings: 0);
                var otherNames = _titleNames.TryGetValue(titleId, out var fetchedNames)
                    ? fetchedNames : new List<TitleName>();
                
                //Create object containing basic information, director information and rating information
                var parsedTitle = new Title(titleId, titleType, primaryTitle, originalTitle, (IReadOnlyCollection<TitleName>) otherNames, 
                    startYear, endYear, runtime, genres, directors, ratings.AverageRating, ratings.NumberOfRatings);
                
                writer.WriteLine(parsedTitle.ToJson());
            }
        }
    }
}