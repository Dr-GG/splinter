using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Parameters.Collapse;
using Splinter.NanoTypes.Domain.Parameters.Dispose;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Platform;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Registry;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Superposition;
using Splinter.NanoTypes.Interfaces.Services.Superposition;
using Splinter.NanoTypes.Interfaces.ServiceScope;

namespace Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

/// <summary>
/// The base and core interface of all agents in Splinters.
/// </summary>
public interface INanoAgent
{
    /// <summary>
    /// The value indicating if the INanoAgent instance has no INanoTable.
    /// </summary>
    bool HasNoNanoTable { get; }

    /// <summary>
    /// The value indicating if the INanoAgent instance has an INanoTable.
    /// </summary>
    bool HasNanoTable { get; }

    /// <summary>
    /// The value indicating if the INanoAgent instance has no parent Tera Agent.
    /// </summary>
    bool HasNoTeraParent { get; }

    /// <summary>
    /// The value indicating if the INanoAgent instance has a parent Tera Agent.
    /// </summary>
    bool HasTeraParent { get; }

    /// <summary>
    /// The Nano Type id.
    /// </summary>
    SplinterId TypeId { get; }

    /// <summary>
    /// The Nano Instance id.
    /// </summary>
    SplinterId InstanceId { get; }

    /// <summary>
    /// The holon type.
    /// </summary>
    HolonType HolonType { get; }

    /// <summary>
    /// The ITeraAgent parent, if any.
    /// </summary>
    ITeraAgent TeraParent { get; set; }

    /// <summary>
    /// The INanoTable reference.
    /// </summary>
    INanoTable NanoTable { get; }

    /// <summary>
    /// The IServiceScope instance.
    /// </summary>
    IServiceScope Scope { get; }

    /// <summary>
    /// A reference to the ISuperpositionAgent.
    /// </summary>
    ISuperpositionAgent SuperpositionAgent { get; }

    /// <summary>
    /// A reference to the ITeraPlatformAgent.
    /// </summary>
    ITeraPlatformAgent TeraPlatformAgent { get; }

    /// <summary>
    /// A reference to the ITeraRegistryAgent.
    /// </summary>
    ITeraRegistryAgent TeraRegistryAgent { get; }

    /// <summary>
    /// A reference to the ITeraMessageAgent.
    /// </summary>
    ITeraMessageAgent TeraMessageAgent { get; }

    /// <summary>
    /// Initialises the INanoAgent.
    /// </summary>
    Task Initialise(NanoInitialisationParameters parameters);

    /// <summary>
    /// Collapses a Nano Type to a Nano Instance.
    /// </summary>
    Task<INanoAgent?> Collapse(NanoCollapseParameters parameters);

    /// <summary>
    /// Locks the agent.
    /// </summary>
    Task Lock();

    /// <summary>
    /// Unlocks the agent.
    /// </summary>
    Task Unlock();

    /// <summary>
    /// Syncs the current state of the agent with an existing agent.
    /// </summary>
    Task Synch(INanoAgent nanoAgent);

    /// <summary>
    /// Disposes the agent.
    /// </summary>
    Task Dispose(NanoDisposeParameters parameters);
}