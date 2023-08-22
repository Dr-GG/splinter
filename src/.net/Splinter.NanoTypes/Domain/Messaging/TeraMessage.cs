using System;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoTypes.Domain.Messaging;

/// <summary>
/// The data structure of a message between Tera Agents.
/// </summary>
public record TeraMessage
{
    /// <summary>
    /// The unique ID of the TeraMessage.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// The message code of the TeraMessage.
    /// </summary>
    public int Code { get; init; }

    /// <summary>
    /// The priority of the message.
    /// </summary>
    /// <remarks>
    /// A higher value is a higher priority.
    /// </remarks>
    public int Priority { get; init; }

    /// <summary>
    /// The number of times this message has been dequeued.
    /// </summary>
    public int DequeueCount { get; init; }

    /// <summary>
    /// The current state of the message.
    /// </summary>
    public TeraMessageStatus Status { get; init; }

    /// <summary>
    /// The error code attached to the TeraMessage, if any.
    /// </summary>
    public TeraMessageErrorCode? ErrorCode { get; init; }

    /// <summary>
    /// The ID of the Tera Agent the TeraMessage originates from.
    /// </summary>
    public Guid SourceTeraId { get; init; }

    /// <summary>
    /// The ID of the batch that the message belongs to.
    /// </summary>
    public Guid BatchId { get; init; }

    /// <summary>
    /// The content or message.
    /// </summary>
    public string? Message { get; set; } = string.Empty; // Keep this a set for performance reasons.

    /// <summary>
    /// The error message attached to the message, if any.
    /// </summary>
    public string? ErrorMessage { get; init; } = string.Empty;

    /// <summary>
    /// The error stack trace of the message, if any.
    /// </summary>
    public string? ErrorStacktrace { get; init; } = string.Empty;

    /// <summary>
    /// The timestamp of when the message will expire.
    /// </summary>
    public DateTime AbsoluteExpiryTimestamp { get; init; }

    /// <summary>
    /// The timestamp of when the message was logged.
    /// </summary>
    public DateTime LoggedTimestamp { get; init; }

    /// <summary>
    /// The timestamp of when the message was last dequeued.
    /// </summary>
    public DateTime? DequeuedTimestamp { get; init; }

    /// <summary>
    /// The timestamp of when the message was processed.
    /// </summary>
    public DateTime? CompletedTimestamp { get; init; }
}