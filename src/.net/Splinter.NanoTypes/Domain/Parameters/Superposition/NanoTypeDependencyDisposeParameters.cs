using System;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoTypes.Domain.Parameters.Superposition;

public record NanoTypeDependencyDisposeParameters : NanoParameters
{
    public Guid TeraId { get; init; }
    public SplinterId NanoType { get; init; } = null!;
}