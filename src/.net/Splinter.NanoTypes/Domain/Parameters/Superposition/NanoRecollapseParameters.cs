using System;
using System.Collections.Generic;
using System.Linq;

namespace Splinter.NanoTypes.Domain.Parameters.Superposition;

/// <summary>
/// The nano parameters used to recollapse Nano Types.
/// </summary>
public record NanoRecollapseParameters : NanoParameters
{
    /// <summary>
    /// The ID of the Tera Agent which initiated the recollapse.
    /// </summary>
    public Guid? SourceTeraId { get; init; }

    /// <summary>
    /// The collection of ID's Tera Agent instances to be targeted for the recollapse.
    /// </summary>
    public IEnumerable<Guid> TeraIds { get; init; } = Enumerable.Empty<Guid>();
}