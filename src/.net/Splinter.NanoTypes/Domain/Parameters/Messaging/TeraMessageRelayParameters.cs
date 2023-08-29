using System;
using System.Collections.Generic;
using System.Linq;

namespace Splinter.NanoTypes.Domain.Parameters.Messaging;

/// <summary>
/// The nano parameters used to relay a TeraMessage.
/// </summary>
public record TeraMessageRelayParameters : INanoParameters
{
    /// <summary>
    /// The code of the TeraMessage.
    /// </summary>
    public int Code { get; init; }

    /// <summary>
    /// The priority of the TeraMessage.
    /// </summary>
    /// <remarks>
    /// A higher value indicates a higher priority.
    /// </remarks>
    public int Priority { get; init; }

    /// <summary>
    /// The message content.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// The timestamp of when the TeraMessage will expire.
    /// </summary>
    public TimeSpan? AbsoluteExpiryTimeSpan { get; init; }

    /// <summary>
    /// The ID of the Tera Agent the message originates from.
    /// </summary>
    public Guid SourceTeraId { get; init; }

    /// <summary>
    /// The ID's of Tera Agents that will receive the message.
    /// </summary>
    public IEnumerable<Guid> RecipientTeraIds { get; init; } = Enumerable.Empty<Guid>();
}