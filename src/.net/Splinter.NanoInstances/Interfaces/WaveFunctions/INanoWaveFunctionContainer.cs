using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Interfaces.WaveFunctions;

/// <summary>
/// The interface that acts a repository of matched Nano Type and Nano Instances.
/// </summary>
public interface INanoWaveFunctionContainer
{
    /// <summary>
    /// Attempts to collapse a Nano Type to a Nano Instance based on specified parameters.
    /// </summary>
    Task<INanoAgent?> Collapse(NanoCollapseParameters collapseParameters);

    /// <summary>
    /// Synchronises or updates the Nano Instance Type associated with a Nano Type ID. 
    /// </summary>
    Task Synch(Guid nanoTypeId, Type type);
}