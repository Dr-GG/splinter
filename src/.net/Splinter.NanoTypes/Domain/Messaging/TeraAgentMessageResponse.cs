using System;

namespace Splinter.NanoTypes.Domain.Messaging
{
    public record TeraAgentMessageResponse
    {
        public Guid TeraId { get; init; }
        public long MessageId { get; init; }
    }
}
