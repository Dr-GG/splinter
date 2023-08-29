using System;

namespace Splinter.NanoTypes.Domain.Parameters.Knowledge;

/// <summary>
/// The nano parameters used when executing a cycle in a Tera Agent.
/// </summary>
public record TeraAgentExecutionParameters : INanoParameters
{
    /// <summary>
    /// The current execution count.
    /// </summary>
    public long ExecutionCount { get; init; }

    /// <summary>
    /// The timestamp of when the first execution took place.
    /// </summary>
    public DateTime AbsoluteTimestamp { get; init; }

    /// <summary>
    /// The current timestamp.
    /// </summary>
    public DateTime RelativeTimestamp { get; init; } 

    /// <summary>
    /// The amount of elapsed time since the first execution took place.
    /// </summary>
    public TimeSpan AbsoluteTimeElapsed { get; init; }

    /// <summary>
    /// The amount of elapsed time since the last execution took place.
    /// </summary>
    public TimeSpan RelativeTimeElapsed { get; init; }
}