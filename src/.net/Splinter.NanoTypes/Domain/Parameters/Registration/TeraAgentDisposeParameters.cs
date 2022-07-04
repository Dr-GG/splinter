using System;

namespace Splinter.NanoTypes.Domain.Parameters.Registration;

public record TeraAgentDisposeParameters : NanoParameters
{
    public Guid TeraId { get; init; }
}