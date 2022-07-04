using System;
using Splinter.NanoTypes.Default.Domain.Superposition;
using Splinter.NanoTypes.Domain.Parameters.Superposition;

namespace Splinter.NanoTypes.Default.Domain.Parameters.Superposition;

public record NanoSuperpositionMappingRecollapseParameters : NanoRecollapseParameters
{
    public Guid NanoTypeId { get; init; }
    public SuperpositionMapping SuperpositionMapping { get; init; } = null!;
}