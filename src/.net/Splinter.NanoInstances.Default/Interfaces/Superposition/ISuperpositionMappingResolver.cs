using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoInstances.Default.Models;
using Splinter.NanoTypes.Default.Domain.Superposition;

namespace Splinter.NanoInstances.Default.Interfaces.Superposition;

public interface ISuperpositionMappingResolver
{
    Task<IEnumerable<InternalSuperpositionMapping>> Resolve(IEnumerable<SuperpositionMapping> superpositionMappings);
}