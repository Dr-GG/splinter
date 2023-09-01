using System.Threading.Tasks;
using Splinter.NanoInstances.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Domain.Parameters.Termination;

namespace Splinter.NanoInstances.Tests.Agents;

public class SingletonUnitTestTeraAgent : SingletonTeraAgent
{
    private readonly object _lock = new();

    public override SplinterId TypeId { get; } = new();
    public override SplinterId InstanceId { get; } = new();
    public int AttemptInitCount { get; private set; }
    public int AttemptDisposeCount { get; private set; }
    public int InitCount { get; private set; }
    public int DisposeCount { get; private set; }
    public int ThreadSleep { get; set; }

    public override Task Initialise(NanoInitialisationParameters parameters)
    {
        lock (_lock)
        {
            AttemptInitCount++;
        }

        return base.Initialise(parameters);
    }

    public override Task Terminate(NanoTerminationParameters parameters)
    {
        lock (_lock)
        {
            AttemptDisposeCount++;
        }

        return base.Terminate(parameters);
    }

    protected override async Task SingletonInitialise(NanoInitialisationParameters parameters)
    {
        lock (_lock)
        {
            InitCount++;
        }

        await Task.Delay(ThreadSleep); // This will cause some locks to form.
        await base.SingletonInitialise(parameters);
    }

    protected override async Task SingletonTerminate(NanoTerminationParameters parameters)
    {
        lock (_lock)
        {
            DisposeCount++;
        }

        await Task.Delay(ThreadSleep); // This will cause some locks to form.
        await base.SingletonTerminate(parameters);
    }
}