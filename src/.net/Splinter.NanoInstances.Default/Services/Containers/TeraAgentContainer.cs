using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Exceptions.Containers;
using Splinter.NanoTypes.Domain.Parameters.Containers;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;
using Splinter.NanoTypes.Interfaces.Services.Containers;
using Tenjin.Extensions;

namespace Splinter.NanoInstances.Default.Services.Containers;

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

    public long NumberOfTeraAgents => _teraAgents.Count;

    private TeraAgentContainerExecutionParameters Parameters
    {
        get
        {
            if (_parameters == null)
            {
                throw new TeraAgentContainerNotInitialisedException();
            }

            return _parameters;
        }
    }

    public Task Initialise(TeraAgentContainerExecutionParameters parameters)
    {
        _parameters = parameters;

        return Task.CompletedTask;
    }

    public Task Register(ITeraAgent agent)
    {
        _pendingTeraAgentAdditions.Enqueue(agent);

        return Task.CompletedTask;
    }

    public Task Dispose(ITeraAgent agent)
    {
        _pendingTeraAgentRemovals.Enqueue(agent);

        return Task.CompletedTask;
    }

    public Task Execute()
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

    public Task Halt()
    {
        Dispose();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _running = false;
        _tokenSource.Cancel();
    }

    public ValueTask DisposeAsync()
    {
        Dispose();

        return ValueTask.CompletedTask;
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