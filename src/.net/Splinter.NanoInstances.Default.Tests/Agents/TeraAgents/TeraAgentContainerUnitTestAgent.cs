using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoInstances.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;

namespace Splinter.NanoInstances.Default.Tests.Agents.TeraAgents;

public class TeraAgentContainerUnitTestAgent : TeraAgent
{
    private int _executionCounter;

    private readonly int _executionLimit;

    public static readonly object LockRoot = new();
    public static int ExecutionHit;
    public static int CompletedHit;
    public static IDictionary<int, int> ThreadIdCounter = new Dictionary<int, int>();

    public static void ResetMultiThreadVariables()
    {
        ExecutionHit = 0;
        CompletedHit = 0;
        ThreadIdCounter.Clear();
    }

    public TeraAgentContainerUnitTestAgent(int executionLimit = int.MaxValue)
    {
        _executionLimit = executionLimit;
    }

    public override SplinterId TypeId => throw new NotSupportedException();
    public override SplinterId InstanceId => throw new NotSupportedException();
    public ICollection<TeraAgentExecutionParameters> ExecutionParameters { get; } = new List<TeraAgentExecutionParameters>();

    public override Task Execute(TeraAgentExecutionParameters parameters)
    {
        if (parameters.ExecutionCount > _executionLimit)
        {
            return Task.CompletedTask;
        }

        ExecutionParameters.Add(parameters);

        lock (LockRoot)
        {
            ExecutionHit++;
            _executionCounter++;

            if (_executionCounter == _executionLimit)
            {
                CompletedHit++;
            }

            var threadId = System.Environment.CurrentManagedThreadId;

            if (ThreadIdCounter.TryGetValue(threadId, out var count))
            {
                count++;
            }
            else
            {
                count = 1;
            }

            ThreadIdCounter[threadId] = count;
        }

        return Task.CompletedTask;
    }
}