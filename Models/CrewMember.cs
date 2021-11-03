namespace IMDBEnricher.Models
{
    /// <summary>
    /// Stripped down representation of IMDB crew-/cast-model
    /// </summary>
    public class CrewMember
    {
        public CrewMember(string crewId, string primaryName)
        {
            CrewId = crewId;
            PrimaryName = primaryName;
        }
        
        //Unique ID used on IMDB to distinguish each person
        public string CrewId { get; set; }
        //Primary name used on IMDB to refer to the person
        public string PrimaryName { get; set; }
    }
}