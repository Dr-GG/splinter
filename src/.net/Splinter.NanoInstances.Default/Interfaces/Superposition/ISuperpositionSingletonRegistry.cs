using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Default.Interfaces.Superposition;

/// <summary>
/// The interfaces that acts as a registry of INanoAgent instances that act as singletons.
/// </summary>
public interface ISuperpositionSingletonRegistry : IAsyncDisposable
{
    /// <summary>
    /// Registers a single INanoAgent as a singleton.
    /// </summary>
    Task<INanoAgent> Register(Guid nanoTypeId, INanoAgent singleton);

    /// <summary>
    /// Attempts to fetch the singleton instance based on a Nano Type ID.
    /// </summary>
    Task<INanoAgent?> Fetch(Guid nanoTypeId);
}