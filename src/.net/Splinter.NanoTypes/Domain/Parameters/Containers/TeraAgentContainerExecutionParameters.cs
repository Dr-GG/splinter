using System;

namespace Splinter.NanoTypes.Domain.Parameters.Containers;

/// <summary>
/// The nano parameters when executing an ITeraAgentContainer.
/// </summary>
public record TeraAgentContainerExecutionParameters : NanoParameters
{
    /// <summary>
    /// The number of concurrent threads to be used.
    /// </summary>
    /// <remarks>
    /// If this value is null, it will used the number of logical processors as a thread count.
    /// </remarks>
    public int? NumberOfConcurrentThreads { get; init; }

    /// <summary>
    /// The time span to wait between each execution cycle.
    /// </summary>
    public TimeSpan ExecutionIntervalTimeSpan { get; init; }

    /// <summary>
    /// The DateTime value to start execution from.
    /// </summary>
    /// <remarks>
    /// If the value is null, the current UTC date will be used.
    /// </remarks>
    public DateTime? StartTimestamp { get; init; }

    /// <summary>
    /// The amount of time to add to the start timestamp.
    /// </summary>
    /// <remarks>
    /// If the value is null, no time will be added.
    /// </remarks>
    public TimeSpan? IncrementTimestamp { get; init; }
}