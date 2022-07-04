using System;

namespace Splinter.NanoTypes.Domain.Parameters.Superposition;

public record NanoRecollapseOperationParameters : NanoParameters
{
    public bool IsSuccessful { get; init; }
    public Guid NanoRecollapseOperationId { get; init; }
    public Guid TeraId { get; init; }
}