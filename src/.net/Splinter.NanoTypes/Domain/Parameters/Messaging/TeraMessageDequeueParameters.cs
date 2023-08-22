using System;

namespace Splinter.NanoTypes.Domain.Parameters.Messaging;

/// <summary>
/// The nano parameters when retrieving a number of TeraMessages.
/// </summary>
public record TeraMessageDequeueParameters : NanoParameters
{
    /// <summary>
    /// The ID of the Tera Agent that acts as the recipient.
    /// </summary>
    public Guid TeraId { get; init; }

    /// <summary>
    /// The maximum number of TeraMessages to dequeue.
    /// </summary>
    public int MaximumNumberOfTeraMessages { get; init; }
}