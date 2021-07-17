using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Interfaces.Agents;

namespace Splinter.NanoInstances.Default.Interfaces.Superposition
{
    public interface ISuperpositionSingletonRegistry : IAsyncDisposable
    {
        Task<INanoAgent> Register(Guid nanoTypeId, INanoAgent singleton);
        Task<INanoAgent?> Fetch(Guid nanoTypeId);
    }
}
