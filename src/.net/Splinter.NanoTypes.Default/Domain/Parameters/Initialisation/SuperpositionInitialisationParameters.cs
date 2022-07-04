using System.Collections.Generic;
using System.Linq;
using Splinter.NanoTypes.Default.Domain.Superposition;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;

namespace Splinter.NanoTypes.Default.Domain.Parameters.Initialisation;

public record SuperpositionInitialisationParameters : NanoInitialisationParameters
{
    public IEnumerable<SuperpositionMapping> SuperpositionMappings { get; init; } = Enumerable.Empty<SuperpositionMapping>();
}