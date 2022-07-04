using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Superposition;

namespace Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Superposition;

public interface ISuperpositionAgent : ITeraAgent
{
    Task<Guid?> Recollapse(NanoRecollapseParameters parameters);
    Task Sync(NanoTypeDependencyDisposeParameters parameters);
    Task Sync(NanoRecollapseOperationParameters parameters);
}