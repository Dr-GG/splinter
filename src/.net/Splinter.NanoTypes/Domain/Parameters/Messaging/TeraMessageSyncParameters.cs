using System.Collections.Generic;
using System.Linq;

namespace Splinter.NanoTypes.Domain.Parameters.Messaging;

/// <summary>
/// The nano parameters used to synchronise multiple TeraMessage instances.
/// </summary>
public record TeraMessageSyncParameters : INanoParameters
{
    /// <summary>
    /// The collection of TeraMessageSyncParameters to be synchronised.
    /// </summary>
    public IEnumerable<TeraMessageSyncParameter> Syncs { get; init; } = Enumerable.Empty<TeraMessageSyncParameter>();
}