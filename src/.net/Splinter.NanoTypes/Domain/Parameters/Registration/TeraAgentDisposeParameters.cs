using System;

namespace Splinter.NanoTypes.Domain.Parameters.Registration;

/// <summary>
/// The nano parameters used to register the disposal of a Tera Agent.
/// </summary>
public record TeraAgentDisposeParameters : NanoParameters
{
    /// <summary>
    /// The ID of the Tera Agent to be disposed of.
    /// </summary>
    public Guid TeraId { get; init; }
}