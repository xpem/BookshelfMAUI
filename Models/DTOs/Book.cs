using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DTOs
{
    public class Book
    {
        [Key]
        [Index("IX_LocalIdAndUid", 2)]
        public int LocalId { get; set; }

        /// <summary>
        /// external id
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? Id { get; set; }

        [Index("IX_UserIdAndStatusAndInactive", 1)]
        [Index("IX_LocalIdAndUid", 1)]
        public int UserId { get; set; }

        public string? LocalTempId { get; set; }

        public required string Title { get; set; }

        public string? SubTitle { get; set; }

        public string? Authors { get; set; }

        public int? Volume { get; set; }

        public int? Pages { get; set; }

        public int? Year { get; set; }

        [Index("IX_UserIdAndStatusAndInactive", 2)]
        public Status? Status { get; set; }

        public string? Genre { get; set; }

        public string? Isbn { get; set; }

        public string? Cover { get; set; }

        public string? GoogleId { get; set; }

        public int? Score { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [Index("IX_UserIdAndStatusAndInactive", 3)]
        public bool Inactive { get; set; }
    }

    public enum Status { None, IllRead, Reading, Read, Interrupted }
}
