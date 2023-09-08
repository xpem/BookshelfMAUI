﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models.Books.Historic
{
    public class BookHistoricItem
    {
        [Key]
        public int? LocalId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
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
