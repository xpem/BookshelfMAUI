namespace BookshelfModels.Books
{
    /// <summary>
    /// Book Item for front
    /// </summary>
    public class UIBookItem
    {
        public string? Key { get; set; }
        public string? Title { get; set; }
        public string? SubtitleAndVol { get; set; }
        //public string? AuthorsAndYear { get; set; }
        public string? Authors { get; set; }
        public string? Pages { get; set; }
        public string? Rate { get; set; }

        public string? Cover { get; set; }

        public bool CoverIsVisible => Cover != null;
    }
}
