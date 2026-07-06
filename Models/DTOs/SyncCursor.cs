using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DTOs
{
    /// <summary>
    /// Stores the last server-side timestamp used as a delta-pull anchor for each entity type.
    /// Using the server's own UpdatedAt values (received in pull responses) avoids clock-skew
    /// problems that arise when the device clock is ahead or behind the server.
    /// </summary>
    [Table("SyncCursor")]
    public class SyncCursor
    {
        /// <summary>
        /// Stable entity name used as a primary key (e.g. "Book", "BookHistoric").
        /// </summary>
        [Key]
        [StringLength(50)]
        public required string EntityName { get; set; }

        /// <summary>
        /// The highest UpdatedAt/CreatedAt value seen in the last successful pull response from the server.
        /// Sent as the query parameter on the next delta pull.
        /// </summary>
        public DateTime ServerTimestamp { get; set; }
    }

    public static class SyncCursorKeys
    {
        public const string Book = "Book";
        public const string BookHistoric = "BookHistoric";
    }
}
