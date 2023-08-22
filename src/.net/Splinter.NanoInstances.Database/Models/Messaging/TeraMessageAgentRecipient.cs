using System;
using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoInstances.Database.Models.Messaging;

/// <summary>
/// Depicts a Tera Agent that acts as a recipient for a Tera Message.
/// </summary>
public record TeraMessageAgentRecipient
{
    /// <summary>
    /// The database model ID of the Tera Agent.
    /// </summary>
    public long TeraAgentId { get; init; }

    /// <summary>
    /// The unique Tera Agent ID of the Tera Agent.
    /// </summary>
    public Guid TeraAgentTeraId { get; init; }

    /// <summary>
    /// The current status of the Tera Agent.
    /// </summary>
    public TeraAgentStatus Status { get; init; }
}