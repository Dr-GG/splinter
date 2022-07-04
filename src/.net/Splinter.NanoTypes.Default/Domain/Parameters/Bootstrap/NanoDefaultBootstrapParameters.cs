using System.Collections.Generic;
using System.Linq;
using Splinter.NanoTypes.Domain.Parameters.Bootstrap;

namespace Splinter.NanoTypes.Default.Domain.Parameters.Bootstrap;

public record NanoDefaultBootstrapParameters : NanoBootstrapParameters
{
    public IEnumerable<string> JsonSettingFileNames { get; init; } = Enumerable.Empty<string>();
}