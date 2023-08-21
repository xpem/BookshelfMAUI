namespace Models.Books
{
    public class Book
    {
        public int? Id { get; set; }

        /// <summary>
        /// Temporary local id
        /// </summary>
        public string? LocalTempId { get; set; }

        public string? Title { get; set; }

        public string? SubTitle { get; set; }

        public string? Authors { get; set; }

        public int? Volume { get; set; }

        public int? Pages { get; set; }

        public int? Year { get; set; }

        public Status? Status { get; set; }

        public string? Genre { get; set; }

        public string? Isbn { get; set; }

        public string? Cover { get; set; }

        public string? GoogleId { get; set; }

        public int? Score { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool Inactive { get; set; }
    }

    public enum Status
    {
        None, IllRead, Reading, Read, Interrupted
    }
}
