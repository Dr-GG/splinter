using System;

namespace Splinter.NanoTypes.Domain.Messaging;

/// <summary>
/// The data structure that represents the response one receives when sending a TeraMessage.
/// </summary>
public record TeraAgentMessageResponse
{
    /// <summary>
    /// The ID of the Tera Agent the message was sent to.
    /// </summary>
    public Guid TeraId { get; init; }

    /// <summary>
    /// The ID of the message that was sent.
    /// </summary>
    public long MessageId { get; init; }
}