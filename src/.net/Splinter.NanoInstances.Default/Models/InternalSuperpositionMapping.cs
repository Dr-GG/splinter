using System;
using Splinter.NanoTypes.Default.Domain.Superposition;

namespace Splinter.NanoInstances.Default.Models;

public record InternalSuperpositionMapping : SuperpositionMapping
{
    public Guid NanoTypeId { get; init; }
}