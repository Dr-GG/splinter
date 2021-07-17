using System.Collections.Generic;
using System.Linq;

namespace Splinter.NanoTypes.Domain.Parameters.Messaging
{
    public record TeraMessageSyncParameters : NanoParameters
    {
        public IEnumerable<TeraMessageSyncParameter> Syncs { get; init; } = Enumerable.Empty<TeraMessageSyncParameter>();
    }
}
