namespace Models.Books.Historic
{
    public class BookHistoric
    {
        public int? Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? TypeId { get; set; }

        public string? Type { get; set; }

        public List<BookHistoricItem>? BookHistoricItems { get; set; }

    }
}
