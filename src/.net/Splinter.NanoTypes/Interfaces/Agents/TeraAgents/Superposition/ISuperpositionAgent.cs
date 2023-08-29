using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Superposition;

namespace Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Superposition;

/// <summary>
/// The ITeraAgent that supports the collapse and recollapse of Nano Types to Nano Instances.
/// </summary>
public interface ISuperpositionAgent : ITeraAgent
{
    /// <summary>
    /// Recollapses previously collapsed Nano Types to new Nano Instances.
    /// </summary>
    Task<Guid?> Recollapse(NanoRecollapseParameters parameters);

    /// <summary>
    /// Synchronises the termination of Nano Type references with regards to a Tera Type.
    /// </summary>
    Task Sync(NanoTypeDependencyTerminationParameters parameters);

    /// <summary>
    /// Synchronises the operational status and progress of recollapsing Nano Types to Nano Instances.
    /// </summary>
    Task Sync(NanoRecollapseOperationParameters parameters);
}