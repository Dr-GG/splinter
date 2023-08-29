using System;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoTypes.Domain.Parameters.Messaging;

/// <summary>
/// Synchronises or updates the state of an existing TeraMessage.
/// </summary>
public record TeraMessageSyncParameter : INanoParameters
{
    /// <summary>
    /// The ID of the TeraMessage to be synced.
    /// </summary>
    public long TeraMessageId { get; init; }

    /// <summary>
    /// The error code, if any.
    /// </summary>
    /// <remarks>
    /// If this property is not null, it will default the TeraMessage into a failed state.
    /// </remarks>
    public TeraMessageErrorCode? ErrorCode { get; init; }

    /// <summary>
    /// The error message.
    /// </summary>
    /// <remarks>
    /// If this property is not null, it will default the TeraMessage into a failed state.
    /// </remarks>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// The error stack trace.
    /// </summary>
    /// <remarks>
    /// If this property is not null, it will default the TeraMessage into a failed state.
    /// </remarks>
    public string? ErrorStackTrace { get; init; }

    /// <summary>
    /// The timestamp of when the TeraMessage was processed.
    /// </summary>
    public DateTime CompletionTimestamp { get; init; } = DateTime.UtcNow;
}