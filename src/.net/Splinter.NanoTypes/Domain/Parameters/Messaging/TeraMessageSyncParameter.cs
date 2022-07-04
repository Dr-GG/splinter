using System;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoTypes.Domain.Parameters.Messaging;

public record TeraMessageSyncParameter : NanoParameters
{
    public long TeraMessageId { get; init; }
    public TeraMessageErrorCode? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }
    public string? ErrorStackTrace { get; init; }
    public DateTime CompletionTimestamp { get; init; } = DateTime.UtcNow;
}