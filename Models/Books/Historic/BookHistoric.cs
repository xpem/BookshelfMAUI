namespace BookshelfModels.Books.Historic
{
    public class BookHistoric
    {
        public int? Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Type { get; set; }

        public BookHistoricItem? BookHistoricItem { get; set; }

    }
}
