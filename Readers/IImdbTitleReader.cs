using IMDBEnricher.Models;

namespace IMDBEnricher.Readers
{
    public interface IImdbTitleReader
    {
        bool OpenFile();
        
        bool ReadImdbTitle(out Title? title);

        bool Reset();
    }
}