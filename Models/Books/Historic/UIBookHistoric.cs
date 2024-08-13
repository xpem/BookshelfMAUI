namespace Models.Books.Historic
{
    public record UIBookHistoric
    {
        public int Id { get; set; }

        public string? HistoricDate { get; set; }

        public string? BookHistoricText { get; set; }

        public string? BookHistoricIcon { get; set; }

        public string? BookTitle { get; set; }
    }
}
