using System;

namespace Splinter.NanoTypes.Domain.Parameters.Containers
{
    public record TeraAgentContainerExecutionParameters : NanoParameters
    {
        public int? NumberOfConcurrentThreads { get; set; }
        public TimeSpan ExecutionIntervalTimeSpan { get; init; }
        public DateTime? StartTimestamp { get; set; }
        public TimeSpan? IncrementTimestamp { get; set; }
    }
}
