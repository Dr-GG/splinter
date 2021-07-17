using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Interfaces.Agents;

namespace Splinter.NanoTypes.Interfaces.WaveFunctions
{
    public interface INanoWaveFunction
    {
        Task<INanoAgent?> Collapse(NanoCollapseParameters parameters);
    }
}
