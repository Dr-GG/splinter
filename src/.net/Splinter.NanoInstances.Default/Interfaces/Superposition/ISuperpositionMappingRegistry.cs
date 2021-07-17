using System;
using System.Threading.Tasks;
using Splinter.NanoInstances.Default.Models;

namespace Splinter.NanoInstances.Default.Interfaces.Superposition
{
    public interface ISuperpositionMappingRegistry
    {
        Task Register(params InternalSuperpositionMapping[] mappings);
        Task<InternalSuperpositionMapping?> Fetch(Guid nanoTypeId);
        Task Synch(InternalSuperpositionMapping superpositionMapping);
    }
}
