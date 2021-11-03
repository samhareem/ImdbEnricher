namespace IMDBEnricher.Models
{
    /// <summary>
    /// Stripped down representation of IMDB title-model
    /// </summary>
    public class TitleName
    {
        public TitleName(string titleId, string name)
        {
            TitleId = titleId;
            Name = name;
        }
        
        //Unique ID used on IMDB to distinguish each title
        public string TitleId { get; set; }
        //Media type, for example movie, short or tvSeries
        public string Name { get; set; }
    }
}