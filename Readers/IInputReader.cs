using System.Collections.Generic;
using IMDBEnricher.Models;

namespace IMDBEnricher.Readers
{
    public interface IInputReader
    {
        bool ReadSearchTitles(string path, out Dictionary<int, SearchTitle> searchTitles);
    }
}