using System.Collections.Generic;

namespace IMDBEnricher.Models
{
    /// <summary>
    /// Stripped down representation of IMDB title-model
    /// </summary>
    public class Title
    {
        public Title(string titleId, string titleType, string primaryName, string originalName, 
            IReadOnlyCollection<TitleName> otherNames, int? startYear, int? endYear, int? runtimeMinutes, 
            string genres, IReadOnlyCollection<CrewMember> directors, float? averageRating, int numberOfRatings)
        {
            TitleId = titleId;
            TitleType = titleType;
            PrimaryName = primaryName;
            OriginalName = originalName;
            OtherNames = otherNames;
            StartYear = startYear;
            EndYear = endYear;
            RuntimeMinutes = runtimeMinutes;
            Genres = genres;
            Directors = directors;
            AverageRating = averageRating;
            NumberOfRatings = numberOfRatings;
        }
        
        //Unique ID used on IMDB to distinguish each title
        public string TitleId { get; set; }
        //Media type, for example movie, short or tvSeries
        public string TitleType { get; set; }
        //Primary name used for the title on IMDB
        public string PrimaryName { get; set; }
        //Original name for the title
        public string OriginalName { get; set; }
        //Other known names for the title
        public IReadOnlyCollection<TitleName> OtherNames { get; set; }
        //First year of publication
        public int? StartYear { get; set; }
        //Final year of publication
        public int? EndYear { get; set; }
        //Runtime in minutes
        public int? RuntimeMinutes { get; set; }
        //Genres assigned to the title
        public string Genres { get; set; }
        //Directors assigned to the title
        public IReadOnlyCollection<CrewMember> Directors { get; set; }
        //Average rating for the title
        public float? AverageRating { get; set; }
        //Total number of ratings for the title
        public int NumberOfRatings { get; set; }
    }
}