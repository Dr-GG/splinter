using System;
using System.Collections.Generic;
using System.Linq;

namespace Splinter.NanoTypes.Domain.Messaging
{
    public record TeraMessageResponse
    {
        public Guid BatchId { get; set; }
        public IEnumerable<Guid> TeraIdsNotFounds { get; set; } = Enumerable.Empty<Guid>();
        public IEnumerable<Guid> TeraIdsDisposed { get; set; } = Enumerable.Empty<Guid>();
        public IEnumerable<TeraAgentMessageResponse> TeraAgentMessageIds { get; set; } = Enumerable.Empty<TeraAgentMessageResponse>();
    }
}
