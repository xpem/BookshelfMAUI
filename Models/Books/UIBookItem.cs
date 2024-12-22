namespace Models.Books
{
    /// <summary>
    /// Book Item for front
    /// </summary>
    public class UIBookItem
    {
        public int Id { get; set; }

        //public string? Key { get; set; }

        public string? Title { get; set; }

        public string? SubtitleAndVol { get; set; }

        //public string? AuthorsAndYear { get; set; }

        public string? Authors { get; set; }

        public string? Pages { get; set; }

        public int Rate { get; set; }

        public bool RateIsVisible => Rate > 0;

        public string? Cover { get; set; }

        public bool CoverIsVisible => Cover != null;
    }
}
