using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoTypes.Domain.Platforms;

/// <summary>
/// A data structure that holds OS information.
/// </summary>
public record OperatingSystemInformation
{
    /// <summary>
    /// The processor architecture.
    /// </summary>
    public ProcessorArchitecture ProcessorArchitecture { get; init; }

    /// <summary>
    /// The OS type.
    /// </summary>
    public OperatingSystem Type { get; init; }

    /// <summary>
    /// A logical description of the OS.
    /// </summary>
    public string Description { get; init; } = string.Empty;
}