using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DTOs
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

        public static string BuildStatusText(int statusId) =>
            statusId switch
            {
                0 => "Nenhum",
                1 => "Vou ler",
                2 => "Lendo",
                3 => "Lido",
                4 => "Interrompido",
                _ => "Desconhecido",
            };
    }
}
