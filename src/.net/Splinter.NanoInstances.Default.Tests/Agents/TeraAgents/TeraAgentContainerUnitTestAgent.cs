using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Splinter.NanoInstances.Agents.TeraAgents;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;

namespace Splinter.NanoInstances.Default.Tests.Agents.TeraAgents
{
    public class TeraAgentContainerUnitTestAgent : TeraAgent
    {
        private readonly int _executionLimit;

        public static readonly object LockRoot = new();
        public static int ExecutionHit;

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
            }

            return Task.CompletedTask;
        }
    }
}
