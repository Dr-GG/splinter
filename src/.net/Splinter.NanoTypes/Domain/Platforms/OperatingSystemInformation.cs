using Splinter.NanoTypes.Domain.Enums;

namespace Splinter.NanoTypes.Domain.Platforms;

public record OperatingSystemInformation
{
    public ProcessorArchitecture ProcessorArchitecture { get; init; }
    public OperatingSystem Type { get; init; }
    public string Description { get; init; } = string.Empty;
}