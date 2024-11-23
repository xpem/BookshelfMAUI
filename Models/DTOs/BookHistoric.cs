using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models.DTOs
{
    public class BookHistoric
    {
        [Key]
        public int? LocalId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public required int? Id { get; set; }

        public int? Uid { get; set; }

        public DateTime? CreatedAt { get; set; }

        public required int BookId { get; set; }

        public int? TypeId { get; set; }

        public string? TypeName { get; set; }

        public List<BookHistoricItem>? BookHistoricItems { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string? BookTitle { get; set; }
    }
}
