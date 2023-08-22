using System;
using System.Collections.Generic;
using System.Linq;

namespace Splinter.NanoTypes.Domain.Messaging;

/// <summary>
/// The response generated when attempting to relay a TeraMessage.
/// </summary>
public record TeraMessageResponse
{
    /// <summary>
    /// The ID of the batch that the TeraMessage belongs to.
    /// </summary>
    public Guid BatchId { get; init; }

    /// <summary>
    /// The collection of Tera Agent ID's that could not be found.
    /// </summary>
    public IEnumerable<Guid> TeraIdsNotFounds { get; init; } = Enumerable.Empty<Guid>();

    /// <summary>
    /// The collection of Tera Agent ID"s that were disposed.
    /// </summary>
    public IEnumerable<Guid> TeraIdsDisposed { get; init; } = Enumerable.Empty<Guid>();

    /// <summary>
    /// The individual TeraAgentMessageResponses from each relayed TeraMessage.
    /// </summary>
    public IEnumerable<TeraAgentMessageResponse> TeraAgentMessageIds { get; init; } = Enumerable.Empty<TeraAgentMessageResponse>();
}