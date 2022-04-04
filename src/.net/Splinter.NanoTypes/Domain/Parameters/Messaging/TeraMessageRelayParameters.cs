using System;
using System.Collections.Generic;
using System.Linq;

namespace Splinter.NanoTypes.Domain.Parameters.Messaging
{
    public record TeraMessageRelayParameters : NanoParameters
    {
        public int Code { get; init; }
        public int Priority { get; init; }
        public string? Message { get; init; }
        public TimeSpan? AbsoluteExpiryTimeSpan { get; init; }
        public Guid SourceTeraId { get; init; }
        public IEnumerable<Guid> RecipientTeraIds { get; init; } = Enumerable.Empty<Guid>();
    }
}
