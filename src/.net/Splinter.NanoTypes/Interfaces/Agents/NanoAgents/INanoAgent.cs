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

public interface INanoAgent
{
    bool HasNoNanoTable { get; }
    bool HasNanoTable { get; }
    bool HasNoTeraParent { get; }
    bool HasTeraParent { get; }
    SplinterId TypeId { get; }
    SplinterId InstanceId { get; }
    HolonType HolonType { get; }
    ITeraAgent TeraParent { get; set; }
    INanoTable NanoTable { get; }
    IServiceScope Scope { get; }
    ISuperpositionAgent SuperpositionAgent { get; }
    ITeraPlatformAgent TeraPlatformAgent { get; }
    ITeraRegistryAgent TeraRegistryAgent { get; }
    ITeraMessageAgent TeraMessageAgent { get; }

    Task Initialise(NanoInitialisationParameters parameters);
    Task<INanoAgent?> Collapse(NanoCollapseParameters parameters);
    Task Lock();
    Task Unlock();
    Task Synch(INanoAgent nanoAgent);
    Task Dispose(NanoDisposeParameters parameters);
}