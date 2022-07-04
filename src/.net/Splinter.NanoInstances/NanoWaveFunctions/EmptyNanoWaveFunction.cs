using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.WaveFunctions;

namespace Splinter.NanoInstances.NanoWaveFunctions;

public class EmptyNanoWaveFunction : INanoWaveFunction
{
    public Task<INanoAgent?> Collapse(NanoCollapseParameters parameters)
    {
        return Task.FromResult<INanoAgent?>(null);
    }
}