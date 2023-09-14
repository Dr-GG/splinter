using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoTypes.Default.Domain.Superposition;
using Splinter.NanoTypes.Domain.Parameters.Superposition;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.WaveFunctions;

namespace Splinter.NanoInstances.Default.Interfaces.NanoWaveFunctions;

/// <summary>
/// The interface that extends the INanoWaveFunction and is used by the ISuperpositionAgent.
/// </summary>
public interface ISuperpositionNanoWaveFunction : INanoWaveFunction, IAsyncDisposable
{
    /// <summary>
    /// Initialises the wave function based on a collection of SuperpositionMapping instances.
    /// </summary>
    Task Initialise(ITeraAgent parent, IEnumerable<SuperpositionMapping> superpositionMappings);

    /// <summary>
    /// Recollapses a previously collapsed Nano Type based on specified parameters.
    /// </summary>
    Task<Guid?> Recollapse(NanoRecollapseParameters parameters);
}