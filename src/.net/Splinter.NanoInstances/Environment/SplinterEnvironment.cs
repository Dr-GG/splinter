using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Platform;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Registry;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Superposition;
using Splinter.NanoTypes.Interfaces.Services.Containers;

namespace Splinter.NanoInstances.Environment;

/// <summary>
/// A collection of static variables that describes a Splinter environment.
/// </summary>
public static class SplinterEnvironment
{
    /// <summary>
    /// The current status of the Splinter environment.
    /// </summary>
    public static SplinterEnvironmentStatus Status { get; set; } = SplinterEnvironmentStatus.Uninitialised;

    /// <summary>
    /// The ITeraAgentContainer instance of the Splinter environment.
    /// </summary>
    public static ITeraAgentContainer TeraAgentContainer { get; set; } = null!;

    /// <summary>
    /// The ISuperpositionAgent instance of the Splinter environment.
    /// </summary>
    public static ISuperpositionAgent SuperpositionAgent { get; set; } = null!;

    /// <summary>
    /// The ITeraPlatformAgent instance of the Splinter environment.
    /// </summary>
    public static ITeraPlatformAgent TeraPlatformAgent { get; set; } = null!;

    /// <summary>
    /// The ITeraRegistryAgent instance of the Splinter environment.
    /// </summary>
    public static ITeraRegistryAgent TeraRegistryAgent { get; set; } = null!;

    /// <summary>
    /// The ITeraMessageAgent instance of the Splinter environment.
    /// </summary>
    public static ITeraMessageAgent TeraMessageAgent { get; set; } = null!;
}