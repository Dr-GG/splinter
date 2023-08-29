using System.Threading;
using System.Threading.Tasks;
using Splinter.NanoInstances.Environment;
using Splinter.NanoTypes.Domain.Enums;
using Splinter.NanoTypes.Domain.Parameters.Initialisation;
using Splinter.NanoTypes.Domain.Parameters.Termination;

namespace Splinter.NanoInstances.Agents.TeraAgents;

/// <summary>
/// The base class for all Tera Agents that are singletons by default.
/// </summary>
public abstract class SingletonTeraAgent : TeraAgent
{
    private readonly SemaphoreSlim _lock = new(1, 1);

    private bool _terminated;
    private bool _initialised;

    /// <inheritdoc />
    public override async Task Initialise(NanoInitialisationParameters parameters)
    {
        if (_initialised)
        {
            return;
        }

        try
        {
            await _lock.WaitAsync();

            if (_initialised)
            {
                return;
            }

            await SingletonInitialise(parameters);
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc />
    public override async Task Terminate(NanoTerminationParameters parameters)
    {
        if (parameters.Force || SplinterEnvironment.Status == SplinterEnvironmentStatus.Disposing)
        {
            await InternalTerminate(parameters);
        }
    }

    /// <summary>
    /// Initialises the singleton based on specified parameters.
    /// </summary>
    protected virtual async Task SingletonInitialise(NanoInitialisationParameters parameters)
    {
        await base.Initialise(parameters);

        _initialised = true;
    }

    /// <summary>
    /// Disposes the single based on specified parameters.
    /// </summary>
    protected virtual async Task SingletonTerminate(NanoTerminationParameters parameters)
    {
        await base.Terminate(parameters);

        _terminated = true;
    }

    private async Task InternalTerminate(NanoTerminationParameters parameters)
    {
        if (_terminated)
        {
            return;
        }

        try
        {
            await _lock.WaitAsync();

            if (_terminated)
            {
                return;
            }

            await SingletonTerminate(parameters);
        }
        finally
        {
            _lock.Release();
        }
    }
}