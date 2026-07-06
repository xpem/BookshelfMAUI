namespace Models.Books.GoogleApi
{
    /// <summary>
    /// UI model for book search results (previously Google Books, now Open Library).
    /// Namespace and class name preserved to avoid breaking existing Views and ViewModels.
    /// </summary>
    public class UIGoogleBook
    {
        /// <summary>
        /// Open Library Work key, e.g. "OL27448W"
        /// </summary>
        public string Id { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Authors { get; set; }

        public string Publisher { get; set; }

        public string PublishedDate { get; set; }

        public int PageCount { get; set; }

        public string Thumbnail { get; set; }

        public string Description { get; set; }

        public string Categories { get; set; }
    }
}
