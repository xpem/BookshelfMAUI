﻿namespace BookshelfModels.Books.Historic
{
    public class BookHistoricItem
    {
        public int? Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? BookFieldName { get; set; }

        public string? UpdatedFrom { get; set; }

        public string? UpdatedTo { get; set; }
    }
}