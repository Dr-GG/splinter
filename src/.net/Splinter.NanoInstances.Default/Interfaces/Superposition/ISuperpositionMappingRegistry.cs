using System;
using System.Threading.Tasks;
using Splinter.NanoInstances.Default.Models;

namespace Splinter.NanoInstances.Default.Interfaces.Superposition;

/// <summary>
/// The interface that acts as a registry for superposition mappings.
/// </summary>
public interface ISuperpositionMappingRegistry
{
    /// <summary>
    /// Registers a collection of InternalSuperpositionMapping instances.
    /// </summary>
    Task Register(params InternalSuperpositionMapping[] mappings);

    /// <summary>
    /// Fetches the InternalSuperpositionMapping based on a Nano Type ID.
    /// </summary>
    Task<InternalSuperpositionMapping?> Fetch(Guid nanoTypeId);

    /// <summary>
    /// Synchronises a new InternalSuperpositionMapping instance with an existing one.
    /// </summary>
    Task Synch(InternalSuperpositionMapping superpositionMapping);
}