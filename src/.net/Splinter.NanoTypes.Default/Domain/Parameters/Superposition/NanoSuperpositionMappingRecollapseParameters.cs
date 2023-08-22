using System;
using Splinter.NanoTypes.Default.Domain.Superposition;
using Splinter.NanoTypes.Domain.Parameters.Superposition;

namespace Splinter.NanoTypes.Default.Domain.Parameters.Superposition;

/// <summary>
/// The extended NanoRecollapseParameters used to recollapse a Nano Type to a Nano Instance.
/// </summary>
public record NanoSuperpositionMappingRecollapseParameters : NanoRecollapseParameters
{
    /// <summary>
    /// The ID of the Nano Type to be recollapsed.
    /// </summary>
    public Guid NanoTypeId { get; init; }

    /// <summary>
    /// The new SuperpositionMapping instance to be registered and used.
    /// </summary>
    public SuperpositionMapping SuperpositionMapping { get; init; } = null!;
}