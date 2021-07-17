using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Dispose;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;
using Splinter.NanoTypes.Interfaces.Agents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;

namespace Splinter.NanoInstances.Extensions
{
    public static class NanoAgentExtensions
    {
        public static async Task Dispose(this INanoAgent nanoAgent)
        {
            var disposeParameters = new NanoDisposeParameters();

            await nanoAgent.Dispose(disposeParameters);
        }

        public static async Task Execute(this ITeraAgent teraAgent)
        {
            var now = DateTime.UtcNow;
            var executionParameters = new TeraAgentExecutionParameters
            {
                ExecutionCount = 1,
                AbsoluteTimestamp = now,
                RelativeTimestamp = now
            };

            await teraAgent.Execute(executionParameters);
        }
    }
}
