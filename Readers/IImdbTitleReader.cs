using IMDBEnricher.Models;

namespace IMDBEnricher.Readers
{
    public interface IImdbTitleReader
    {
        bool OpenFile(string? dataFilePath);
        
        bool ReadImdbTitle(out Title? title);
    }
}