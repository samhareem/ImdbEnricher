using IMDBEnricher.Models;

namespace IMDBEnricher.Algorithms
{
    public interface IComparer
    {
        int Compare(SearchTitle searchTitle, Title imdbTitle);
    }
}