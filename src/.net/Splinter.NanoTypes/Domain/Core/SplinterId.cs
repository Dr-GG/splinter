using System;

namespace Splinter.NanoTypes.Domain.Core
{
    public record SplinterId
    {
        public Guid Guid { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Version { get; init; } = "1.0.0";
    }
}
