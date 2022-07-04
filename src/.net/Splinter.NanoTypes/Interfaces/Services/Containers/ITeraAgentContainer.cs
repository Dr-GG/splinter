using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Containers;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;

namespace Splinter.NanoTypes.Interfaces.Services.Containers;

public interface ITeraAgentContainer : IDisposable, IAsyncDisposable
{
    long NumberOfTeraAgents { get; }

    Task Initialise(TeraAgentContainerExecutionParameters parameters);
    Task Register(ITeraAgent agent);
    Task Dispose(ITeraAgent agent);
    Task Execute();
    Task Halt();
}