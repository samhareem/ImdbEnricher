using System.Collections.Generic;
using IMDBEnricher.Models;

namespace IMDBEnricher.Readers
{
    public interface IImdbDataSetUpdater
    {
        void UpdateTitleInformation(string? datasetDirectory, string? dataFilePath);
    }
}