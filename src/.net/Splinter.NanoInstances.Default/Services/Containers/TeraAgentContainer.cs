using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Splinter.NanoInstances.Extensions;
using Splinter.NanoTypes.Domain.Exceptions.Containers;
using Splinter.NanoTypes.Domain.Parameters.Containers;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Services.Containers;
using Tenjin.Extensions;

namespace Splinter.NanoInstances.Default.Services.Containers;

/// <summary>
/// The default implementation of the ITeraAgentContainer interface.
/// </summary>
public class TeraAgentContainer : ITeraAgentContainer
{
    private bool _running;
    private int _executionCount;
    private DateTime? _initialTimestamp;
    private DateTime? _currentRelativeTimestamp;
    private DateTime? _previousRelativeTimestamp;

    private TeraAgentContainerExecutionParameters? _parameters;

    private readonly ICollection<ITeraAgent> _teraAgents = new List<ITeraAgent>();
    private readonly ConcurrentQueue<ITeraAgent> _pendingTeraAgentAdditions = new();
    private readonly ConcurrentQueue<ITeraAgent> _pendingTeraAgentRemovals = new();
    private readonly CancellationTokenSource _tokenSource = new();

    /// <inheritdoc />
    public long NumberOfTeraAgents => _teraAgents.Count;

    private TeraAgentContainerExecutionParameters Parameters => _parameters.AssertReturnGetterValue
        <TeraAgentContainerExecutionParameters, TeraAgentContainerNotInitialisedException>();

    /// <inheritdoc />
    public Task Initialise(TeraAgentContainerExecutionParameters parameters)
    {
        _parameters = parameters;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task Register(ITeraAgent agent)
    {
        _pendingTeraAgentAdditions.Enqueue(agent);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task Deregister(ITeraAgent agent)
    {
        _pendingTeraAgentRemovals.Enqueue(agent);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task Start()
    {
        if (_parameters == null)
        {
            throw new TeraAgentContainerNotInitialisedException();
        }

        if (_running)
        {
            return Task.CompletedTask;
        }

        _running = true;

        var cancellationToken = _tokenSource.Token;
        var runTask = InternalExecute(cancellationToken);

        runTask.Start();

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task Stop()
    {
        Dispose();

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        InternalDispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        InternalDispose();
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    private void InternalDispose()
    {
        _running = false;
        _tokenSource.Cancel();
    }

    private Task InternalExecute(CancellationToken cancellationToken)
    {
        return new Task(() =>
        {
            while (cancellationToken.CanContinue())
            {
                ProcessPendingAdditions();
                ProcessPendingRemovals();

                var executionParameters = GetExecutionParameters();
                var batchSize = Parameters.NumberOfConcurrentThreads ?? System.Environment.ProcessorCount;

                _teraAgents
                    .Batch(batchSize)
                    .Select(a => a.ToFunctionTask(() => ProcessTeraAgents(executionParameters, a)))
                    .RunParallel(cancellationToken);

                Thread.Sleep(Parameters.ExecutionIntervalTimeSpan);
            }
        });
    }

    private static async Task ProcessTeraAgents(
        TeraAgentExecutionParameters executionParameters,
        IEnumerable<ITeraAgent> agents)
    {
        foreach (var agent in agents)
        {
            await agent.Execute(executionParameters);
        }
    }

    private TeraAgentExecutionParameters GetExecutionParameters()
    {
        return new TeraAgentExecutionParameters
        {
            ExecutionCount = ++_executionCount,
            AbsoluteTimestamp = GetAbsoluteDateTime(),
            RelativeTimestamp = GetRelativeTimestamp(),
            AbsoluteTimeElapsed = GetAbsoluteTimeSpan(),
            RelativeTimeElapsed = GetRelativeTimeSpan()
        };
    }

    private DateTime GetAbsoluteDateTime()
    {
        _initialTimestamp ??= Parameters.StartTimestamp ?? DateTime.UtcNow;

        return _initialTimestamp.Value;
    }

    private DateTime GetRelativeTimestamp()
    {
        if (_currentRelativeTimestamp == null)
        {
            _currentRelativeTimestamp ??= GetAbsoluteDateTime();
            _previousRelativeTimestamp = _currentRelativeTimestamp;

            return _previousRelativeTimestamp.Value;
        }

        _previousRelativeTimestamp = _currentRelativeTimestamp;
        _currentRelativeTimestamp = Parameters.IncrementTimestamp == null 
            ? DateTime.UtcNow 
            : _currentRelativeTimestamp.Value.Add(Parameters.IncrementTimestamp.Value);

        return _currentRelativeTimestamp.Value;
    }

    private TimeSpan GetAbsoluteTimeSpan()
    {
        if (_initialTimestamp == null
            || _currentRelativeTimestamp == null)
        {
            return TimeSpan.Zero;
        }

        return _currentRelativeTimestamp.Value - _initialTimestamp.Value;
    }

    private TimeSpan GetRelativeTimeSpan()
    {
        if (_currentRelativeTimestamp == null
            || _previousRelativeTimestamp == null)
        {
            return TimeSpan.Zero;
        }

        return _currentRelativeTimestamp.Value - _previousRelativeTimestamp.Value;
    }

    private void ProcessPendingAdditions()
    {
        if (_pendingTeraAgentAdditions.IsEmpty)
        {
            return;
        }

        while (_pendingTeraAgentAdditions.TryDequeue(out var agent))
        {
            if (!_teraAgents.Contains(agent))
            {
                _teraAgents.Add(agent);
            }
        }
    }

    private void ProcessPendingRemovals()
    {
        if (_pendingTeraAgentRemovals.IsEmpty)
        {
            return;
        }

        while (_pendingTeraAgentRemovals.TryDequeue(out var agent))
        {
            _teraAgents.Remove(agent);
        }
    }
}