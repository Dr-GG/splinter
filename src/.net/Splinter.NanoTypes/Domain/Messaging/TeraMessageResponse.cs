using System;
using System.Collections.Generic;
using System.Linq;

namespace Splinter.NanoTypes.Domain.Messaging
{
    public record TeraMessageResponse
    {
        public Guid BatchId { get; init; }
        public IEnumerable<Guid> TeraIdsNotFounds { get; init; } = Enumerable.Empty<Guid>();
        public IEnumerable<Guid> TeraIdsDisposed { get; init; } = Enumerable.Empty<Guid>();
        public IEnumerable<TeraAgentMessageResponse> TeraAgentMessageIds { get; init; } = Enumerable.Empty<TeraAgentMessageResponse>();
    }
}
