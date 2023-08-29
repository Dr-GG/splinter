using System;

namespace Splinter.NanoTypes.Domain.Parameters.Registration;

/// <summary>
/// The nano parameters used to deregister a Tera Agent.
/// </summary>
public record TeraAgentDeregistrationParameters : INanoParameters
{
    /// <summary>
    /// The ID of the Tera Agent to be disposed of.
    /// </summary>
    public Guid TeraId { get; init; }
}