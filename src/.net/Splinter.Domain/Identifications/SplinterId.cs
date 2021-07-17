using System;

namespace Splinter.Domain.Identifications
{
    public record SplinterId
    {
        public Guid Guid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0.0";
    }
}
