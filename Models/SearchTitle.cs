namespace IMDBEnricher.Models
{
    /// <summary>
    /// Possible information for a title to match with an IMDB title
    /// </summary>
    public class SearchTitle
    {
        public SearchTitle(string? title, int? year, string? director)
        {
            Title = title;
            Year = year;
            Director = director;
        }
        
        //Possible title
        public string? Title { get; set; }
        //Possible year of publication
        public int? Year { get; set; }
        //Possible director
        public string? Director { get; set; }
    }
}