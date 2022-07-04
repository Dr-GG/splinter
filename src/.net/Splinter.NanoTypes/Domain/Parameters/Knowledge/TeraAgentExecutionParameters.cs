using System;

namespace Splinter.NanoTypes.Domain.Parameters.Knowledge;

public record TeraAgentExecutionParameters : NanoParameters
{
    public long ExecutionCount { get; init; }
    public DateTime AbsoluteTimestamp { get; init; }
    public DateTime RelativeTimestamp { get; init; } 
    public TimeSpan AbsoluteTimeElapsed { get; init; }
    public TimeSpan RelativeTimeElapsed { get; init; }
}