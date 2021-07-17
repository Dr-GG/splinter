using System;
using System.Collections.Generic;
using Splinter.NanoTypes.Default.Domain.Superposition;

namespace Splinter.NanoTypes.Default.Domain.Settings.Superposition
{
    public class SuperpositionSettings
    {
        public TimeSpan RegistryTimeoutSpan { get; set; }
        public IEnumerable<SuperpositionMapping> Mappings { get; set; } = null!;
    }
}
