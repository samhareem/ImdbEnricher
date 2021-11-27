using System.Linq;

namespace IMDBEnricher.Models
{
    /// <summary>
    /// A class signifying a candidate IMDB title for a search title
    /// </summary>
    public class Candidate
    {
        public Candidate(SearchTitle searchTitle, Title imdbTitle, bool choiceCandidate, int score)
        {
            SearchTitle = searchTitle.Title;
            SearchDirector = searchTitle.Director;
            SearchYear = searchTitle.Year;
            Score = score;
            ChoiceCandidate = choiceCandidate;
            ImdbId = imdbTitle.TitleId;
            ImdbTitle = imdbTitle.PrimaryName;
            ImdbOriginalTitle = imdbTitle.OriginalName;
            ImdbDirectors = imdbTitle.Directors.Select(d => d.PrimaryName).ToArray();
            ImdbYear = imdbTitle.StartYear;
        }
        
        public Candidate(SearchTitle searchTitle)
        {
            SearchTitle = searchTitle.Title;
            SearchDirector = searchTitle.Director;
            SearchYear = searchTitle.Year;
        }
        
        //Title searched
        public string? SearchTitle { get; }
        
        //Director searched
        public string? SearchDirector { get; }
        
        //Year searched
        public int? SearchYear { get; }
        
        //Candidate score
        public int? Score { get; }
        
        //Is choice candidate
        public bool? ChoiceCandidate { get; }
        
        //IMDB ID of candidate
        public string? ImdbId { get; }
        
        //IMDB Title
        public string? ImdbTitle { get; }
        
        //IMDB Original title
        public string? ImdbOriginalTitle { get; }
        
        //IMDB Directors
        public string[] ImdbDirectors { get; }
        
        //IMDB publication year
        public int? ImdbYear { get; }
    }
}