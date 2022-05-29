namespace BookshelfModels.Books
{
    public class Book
    {
        public string? BookKey { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? Authors { get; set; }
        public int Year { get; set; }
        public string? Isbn { get; set; }
        public string? Volume { get; set; }
        public int Pages { get; set; }
        public string? Genre { get; set; }
        public DateTime LastUpdate { get; set; }
        public Rating? Rating { get; set; }
        public Situation Situation { get; set; }
        public bool Inactive { get; set; }
    }

    public class Rating
    {
        public int? Rate { get; set; }
        public string? Comment { get; set; }
    }

    public enum Situation
    {
        None, IllRead, Reading, Read, Interrupted
    }
}
