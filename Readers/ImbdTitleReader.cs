using System;
using System.IO;
using IMDBEnricher.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceStack;
namespace IMDBEnricher.Readers
{
    public class ImdbTitleReader : IImdbTitleReader
    {
        private readonly ILogger<ImdbTitleReader> _logger;
        private string _imdbTitleFilePath;
        private StreamReader? _fileStream;

        public ImdbTitleReader(ILogger<ImdbTitleReader> logger, IConfiguration config)
        {
            _logger = logger;
            _imdbTitleFilePath = Path.GetFullPath(config.GetValue<string>("imdbTitleFilePath"));
        }

        public bool OpenFile(string? dataFilePath)
        {
            //Override configuration if parameter given
            _imdbTitleFilePath = dataFilePath ?? _imdbTitleFilePath;
            
            _logger.LogInformation( $"Attempting to open file {Path.GetFileName(_imdbTitleFilePath)}");

            try
            {
                _fileStream = new StreamReader(Path.GetFullPath(_imdbTitleFilePath));
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Could not imdb title file: {e}");
                return false;
            }
        }

        public bool ReadImdbTitle(out Title? imdbTitle)
        {
            if (_fileStream != null && !_fileStream.EndOfStream)
            {
                var imdbTitleRow = _fileStream.ReadLine();
                imdbTitle = imdbTitleRow.FromJson<Title>();
                return true;
            }

            imdbTitle = null;
            return false;
        }
    }
}