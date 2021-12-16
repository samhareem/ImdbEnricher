using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IMDBEnricher.Algorithms;
using IMDBEnricher.Models;
using IMDBEnricher.Readers;
using Microsoft.Extensions.Configuration;
using ServiceStack;

namespace IMDBEnricher
{
    public class ImdbEnricher
    {
        private readonly IInputReader _inputReader;
        private readonly IImdbTitleReader _imdbTitleReader;
        private readonly IImdbDataSetUpdater _imdbDataSetUpdater;
        private readonly IComparer _comparer;

        private readonly int _maximumCandidateScore;
        private readonly int _maximumChoiceCandidateScore;

        public ImdbEnricher(IInputReader inputReader, IImdbTitleReader imdbTitleReader,
            IImdbDataSetUpdater imdbDataSetUpdater, IComparer comparer, IConfiguration config)
        {
            _inputReader = inputReader;
            _imdbTitleReader = imdbTitleReader;
            _imdbDataSetUpdater = imdbDataSetUpdater;
            _comparer = comparer;
            _maximumCandidateScore = config.GetValue<int>("maximumCandidateScore");
            _maximumChoiceCandidateScore = config.GetValue<int>("maximumChoiceCandidateScore");
        }

        public bool UpdateImdbTitles()
        {
            _imdbDataSetUpdater.UpdateTitleInformation();
            return true;
        }

        public bool EnrichImdbInformation(string inputFile, string outputFilePath)
        {
            using var outputFile = new StreamWriter(Path.GetFullPath(outputFilePath));
            
            _imdbTitleReader.OpenFile();
            _inputReader.ReadSearchTitles(inputFile, out var searchTitles);
            var candidates = new Dictionary<int, List<Candidate>>();
            while (_imdbTitleReader.ReadImdbTitle(out var imdbTitle))
            {
                foreach (var searchTitle in searchTitles)
                {
                    var score = _comparer.Compare(searchTitle.Value, imdbTitle);

                    //If we find a choice candidate, we scrap any other candidates, remove the title from the search and break early
                    if (score <= _maximumChoiceCandidateScore)
                    {
                        candidates[searchTitle.Key] = new List<Candidate>
                            {new Candidate(searchTitle.Value, imdbTitle, true, score)};
                        searchTitles.Remove(searchTitle.Key);
                        break;
                    }

                    //If we find a candidate, add it to the list
                    if (score <= _maximumCandidateScore)
                    {
                        if (candidates.ContainsKey(searchTitle.Key))
                        {
                            candidates[searchTitle.Key].Add(new Candidate(searchTitle.Value, imdbTitle, false, score));
                        }
                        else
                        {
                            candidates[searchTitle.Key] = new List<Candidate>
                                {new Candidate(searchTitle.Value, imdbTitle, false, score)};
                        }
                    }
                }
            }

            //Print candidate lists to output file
            foreach (var candidateList in candidates.Values)
            {
                //Sort list by ascending score
                candidateList.Sort((x, y) => Nullable.Compare(x.Score, y.Score));
                
                //Write candidates to output file ordered by ascending score
                foreach (var candidate in candidateList)
                {
                    outputFile.WriteLine(candidate.ToJson());
                }
            }

            return true;
        }
    }
}       