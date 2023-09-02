namespace Models.Books.Historic
{
    public class BookHistoric
    {
        public required int? Id { get; set; }

        public int? Uid { get; set; }

        public DateTime? CreatedAt { get; set; }

        public required int BookId { get; set; }

        public int? TypeId { get; set; }

        public string? TypeName { get; set; }

        public List<BookHistoricItem>? BookHistoricItems { get; set; }

    }
}
