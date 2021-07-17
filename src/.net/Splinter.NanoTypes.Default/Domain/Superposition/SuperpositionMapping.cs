using Splinter.NanoTypes.Default.Domain.Enums;

namespace Splinter.NanoTypes.Default.Domain.Superposition
{
    public record SuperpositionMapping
    {
        public string? Description { get; init; }
        public SuperpositionMode Mode { get; init; }
        public SuperpositionScope Scope { get; init; }
        public string NanoInstanceType { get; init; } = string.Empty;
    }
}
