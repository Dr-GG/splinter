using System;

namespace Splinter.NanoTypes.Domain.Parameters.Containers;

public record TeraAgentContainerExecutionParameters : NanoParameters
{
    public int? NumberOfConcurrentThreads { get; init; }
    public TimeSpan ExecutionIntervalTimeSpan { get; init; }
    public DateTime? StartTimestamp { get; init; }
    public TimeSpan? IncrementTimestamp { get; init; }
}