using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IMDBEnricher.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IMDBEnricher.Readers
{
    public class InputReader : IInputReader
    {
        private readonly ILogger<InputReader> _logger;
        
        private readonly char _delimiter;
        
        public InputReader(ILogger<InputReader> logger, IConfiguration config)
        {
            _logger = logger;
            _delimiter = config.GetValue<char>("inputFileDelimiter");
        }

        public bool ReadSearchTitles(string path, out Dictionary<int, SearchTitle> searchTitles)
        {
            _logger.LogInformation( $"Attempting to open input file {Path.GetFileName(path)}");
            try
            {
                using var fileStream = new StreamReader(Path.GetFullPath(path));
                var parsedTitles = new Dictionary<int, SearchTitle>();
                var index = 0;
                while (!fileStream.EndOfStream)
                {
                    //Parse search title information
                    var searchTitleInfo = fileStream.ReadLine().Split(_delimiter);
                    var title = searchTitleInfo[0];
                    var year = int.TryParse(searchTitleInfo[1], out var parsedYear)
                        ? (int?) parsedYear : null;
                    var director = searchTitleInfo[2];
                    
                    //Create object containing search title information
                    parsedTitles.Add(index, new SearchTitle(title, year, director));

                    index += 1;
                }

                searchTitles = parsedTitles;
                return true;    
            } catch (Exception e)
            {
                searchTitles = new Dictionary<int, SearchTitle>();
                return false;
            }
            
        }
    }
}