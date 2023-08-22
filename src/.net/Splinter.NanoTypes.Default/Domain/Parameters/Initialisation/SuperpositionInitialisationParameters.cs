using System.Collections.Generic;
using System.Linq;
using Splinter.NanoTypes.Default.Domain.Superposition;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;

namespace Splinter.NanoTypes.Default.Domain.Parameters.Initialisation;

/// <summary>
/// The extended NanoInitialisationParameters used to initialise superposition mappings.
/// </summary>
public record SuperpositionInitialisationParameters : NanoInitialisationParameters
{
    /// <summary>
    /// The collection of SuperpositionMapping instances to be used.
    /// </summary>
    public IEnumerable<SuperpositionMapping> SuperpositionMappings { get; init; } = Enumerable.Empty<SuperpositionMapping>();
}