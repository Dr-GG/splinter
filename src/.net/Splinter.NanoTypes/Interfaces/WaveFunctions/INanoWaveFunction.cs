using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoTypes.Interfaces.WaveFunctions;

/// <summary>
/// The interface that represents a quantum superposition wave function.
/// </summary>
public interface INanoWaveFunction
{
    /// <summary>
    /// Collapses a specified Nano Type to a Nano Instance based on specified parameters.
    /// </summary>
    Task<INanoAgent?> Collapse(NanoCollapseParameters parameters);
}