using System;
using Splinter.NanoTypes.Default.Domain.Superposition;

namespace Splinter.NanoInstances.Default.Models;

/// <summary>
/// An extended SuperpositionMapping used internally by the default implementation of the ISuperpositionAgent.
/// </summary>
public record InternalSuperpositionMapping : SuperpositionMapping
{
    /// <summary>
    /// The Nano Type ID attached to the SuperpositionMapping.
    /// </summary>
    public Guid NanoTypeId { get; init; }
}