using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Interfaces.Agents;

namespace Splinter.NanoInstances.Interfaces.WaveFunctions
{
    public interface INanoWaveFunctionContainer
    {
        Task<INanoAgent?> Collapse(NanoCollapseParameters collapseParameters);
        Task Synch(Guid nanoTypeId, Type type);
    }
}
