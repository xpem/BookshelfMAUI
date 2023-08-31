namespace Models
{
    /// <summary>
    /// Table Versions
    /// </summary>
    public record VersionDbTables
    {
        public required int Id { get; set; }

        public required int VERSION { get; set; }

        //public required int USER { get; set; }

        //public required int BOOK { get; set; }

        //public required int BOOK_HISTORIC { get; set; }

        //public required int BOOK_HISTORIC_ITEM { get; set; }
    }
}