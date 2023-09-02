namespace Models.Books.Historic
{
    public class BookHistoricItem
    {
        public required int Id { get; set; }

        public int BookHistoricId { get; set; }

        public int? Uid { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? BookFieldId { get; set; }

        public string? BookFieldName { get; set; }

        public string? UpdatedFrom { get; set; }

        public string? UpdatedTo { get; set; }
    }
}
