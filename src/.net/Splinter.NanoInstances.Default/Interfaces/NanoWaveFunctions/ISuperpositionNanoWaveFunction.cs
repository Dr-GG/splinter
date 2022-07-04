using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoTypes.Default.Domain.Superposition;
using Splinter.NanoTypes.Domain.Parameters.Superposition;
using Splinter.NanoTypes.Interfaces.WaveFunctions;

namespace Splinter.NanoInstances.Default.Interfaces.NanoWaveFunctions;

public interface ISuperpositionNanoWaveFunction : INanoWaveFunction, IAsyncDisposable
{
    Task Initialise(IEnumerable<SuperpositionMapping> superpositionMappings);
    Task<Guid?> Recollapse(NanoRecollapseParameters parameters);
}