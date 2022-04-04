using System;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoTypes.Domain.Messaging
{
    public record TeraMessage
    {
        public long Id { get; init; }
        public int Code { get; init; }
        public int Priority { get; init; }
        public int DequeueCount { get; init; }
        public TeraMessageStatus Status { get; init; }
        public TeraMessageErrorCode? ErrorCode { get; init; }
        public Guid SourceTeraId { get; init; }
        public Guid BatchId { get; init; }
        public string? Message { get; set; } = string.Empty; // Keep this a set for performance reasons.
        public string? ErrorMessage { get; init; } = string.Empty;
        public string? ErrorStacktrace { get; init; } = string.Empty;
        public DateTime AbsoluteExpiryTimestamp { get; init; }
        public DateTime LoggedTimestamp { get; init; }
        public DateTime? DequeuedTimestamp { get; init; }
        public DateTime? CompletedTimestamp { get; init; }
    }
}
