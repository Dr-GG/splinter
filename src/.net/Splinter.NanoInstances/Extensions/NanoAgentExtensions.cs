using System;
using System.Threading.Tasks;
using Splinter.NanoTypes.Domain.Parameters.Knowledge;
using Splinter.NanoTypes.Domain.Parameters.Termination;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Splinter.NanoTypes.Interfaces.Agents.TeraAgents;

namespace Splinter.NanoInstances.Extensions;

/// <summary>
/// A collection of extension methods for the INanoAgent interface.
/// </summary>
public static class NanoAgentExtensions
{
    /// <summary>
    /// Terminates an INanoAgent instance.
    /// </summary>
    public static async Task Terminate(this INanoAgent nanoAgent)
    {
        var disposeParameters = new NanoTerminationParameters();

        await nanoAgent.Terminate(disposeParameters);
    }

    /// <summary>
    /// Executes an ITeraAgent using default execution parameters.
    /// </summary>
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