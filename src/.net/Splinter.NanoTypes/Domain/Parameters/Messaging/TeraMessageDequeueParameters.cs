using System;

namespace Splinter.NanoTypes.Domain.Parameters.Messaging;

public record TeraMessageDequeueParameters : NanoParameters
{
    public Guid TeraId { get; init; }
    public int MaximumNumberOfTeraMessages { get; init; }
}