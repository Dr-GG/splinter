using System;
using System.Collections.Generic;
using System.Linq;

namespace Splinter.NanoTypes.Domain.Parameters.Superposition;

public record NanoRecollapseParameters : NanoParameters
{
    public Guid? SourceTeraId { get; init; }
    public IEnumerable<Guid> TeraIds { get; init; } = Enumerable.Empty<Guid>();
}