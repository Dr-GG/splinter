using Splinter.NanoTypes.Default.Domain.Enums;

namespace Splinter.NanoTypes.Default.Domain.Superposition;

/// <summary>
/// Depicts a superposition mapping.
/// </summary>
public record SuperpositionMapping
{
    /// <summary>
    /// A logical description of the mapping.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The SuperpositionMode of the mapping.
    /// </summary>
    public SuperpositionMode Mode { get; init; }

    /// <summary>
    /// The lifetime scope of the mapping.
    /// </summary>
    public SuperpositionScope Scope { get; init; }

    /// <summary>
    /// The NanoInstanceType that will be given.
    /// </summary>
    public string NanoInstanceType { get; init; } = string.Empty;
}