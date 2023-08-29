using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Containers;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;

namespace Splinter.NanoTypes.Interfaces.Services.Containers;

/// <summary>
/// The interface that reflects a collection ITeraAgent instances that requires execution.
/// </summary>
public interface ITeraAgentContainer : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the number of registered ITeraAgent instances.
    /// </summary>
    long NumberOfTeraAgents { get; }

    /// <summary>
    /// Initialises the ITeraAgentContainer instance with specified parameters.
    /// </summary>
    Task Initialise(TeraAgentContainerExecutionParameters parameters);

    /// <summary>
    /// Registers a single ITeraAgent instance.
    /// </summary>
    Task Register(ITeraAgent agent);

    /// <summary>
    /// Removes a single ITeraAgent instance.
    /// </summary>
    Task Deregister(ITeraAgent agent);

    /// <summary>
    /// Starts with the execution of registered ITeraAgent instances.
    /// </summary>
    Task Start();

    /// <summary>
    /// Stops the execution of registered ITeraAgent instances.
    /// </summary>
    Task Stop();
}