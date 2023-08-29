using System;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoTypes.Domain.Parameters.Superposition;

/// <summary>
/// The nano parameters that is used to register the termination of Nano Type dependencies in a Tera Agent.
/// </summary>
public record NanoTypeDependencyTerminationParameters : INanoParameters
{
    /// <summary>
    /// The ID of the Tera Agent.
    /// </summary>
    public Guid TeraId { get; init; }

    /// <summary>
    /// The SplinterId of the Nano Type reference being disposed of.
    /// </summary>
    public SplinterId NanoType { get; init; } = null!;
}