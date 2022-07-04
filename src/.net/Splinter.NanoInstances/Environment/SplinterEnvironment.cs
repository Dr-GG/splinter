using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Messaging;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Platform;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Registry;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Superposition;
using Splinter.NanoTypes.Interfaces.Services.Containers;

namespace Splinter.NanoInstances.Environment;

public static class SplinterEnvironment
{
    public static SplinterEnvironmentStatus Status { get; set; } = SplinterEnvironmentStatus.Uninitialised;

    public static ITeraAgentContainer TeraAgentContainer { get; set; } = null!;
    public static ISuperpositionAgent SuperpositionAgent { get; set; } = null!;
    public static ITeraPlatformAgent TeraPlatformAgent { get; set; } = null!;
    public static ITeraRegistryAgent TeraRegistryAgent { get; set; } = null!;
    public static ITeraMessageAgent TeraMessageAgent { get; set; } = null!;
}