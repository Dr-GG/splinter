using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Database.DbContext.Models
{
    [Table("TeraMessages", Schema = "tera")]
    public class TeraMessageModel
    {
        public long Id { get; set; }
        public int Code { get; set; }
        public int Priority { get; set; }
        public int DequeueCount { get; set; }
        public Guid BatchId { get; set; }
        public TeraMessageStatus Status { get; set; }
        public TeraMessageErrorCode? ErrorCode { get; set; }
        public long SourceTeraAgentId { get; set; }
        public TeraAgentModel SourceTeraAgent { get; set; } = null!;
        public long RecipientTeraAgentId { get; set; }
        public TeraAgentModel RecipientTeraAgent { get; set; } = null!;
        [Timestamp]
        public byte[] ETag { get; set; } = null!;
        [MaxLength]
        public string? Message { get; set; }
        [MaxLength]
        public string? ErrorMessage { get; set; }
        [MaxLength]
        public string? ErrorStackTrace { get; set; }
        public DateTime AbsoluteExpiryTimestamp { get; set; }
        public DateTime LoggedTimestamp { get; set; }
        public DateTime? DequeuedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }
        public PendingTeraMessageModel Pending { get; set; } = null!;
    }
}
