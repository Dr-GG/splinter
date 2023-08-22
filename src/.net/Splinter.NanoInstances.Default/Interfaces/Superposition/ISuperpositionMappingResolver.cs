using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoInstances.Default.Models;
using Splinter.NanoTypes.Default.Domain.Superposition;

namespace Splinter.NanoInstances.Default.Interfaces.Superposition;

/// <summary>
/// The interface that resolves a SuperpositionMapping instances to InternalSuperpositionMapping instances.
/// </summary>
public interface ISuperpositionMappingResolver
{
    /// <summary>
    /// Resolves a collection of SuperpositionMapping instances to InternalSuperpositionMapping instances.
    /// </summary>
    Task<IEnumerable<InternalSuperpositionMapping>> Resolve(IEnumerable<SuperpositionMapping> superpositionMappings);
}