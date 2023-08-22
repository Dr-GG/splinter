using System.Collections.Generic;
using System.Linq;
using Splinter.NanoTypes.Domain.Parameters.Bootstrap;

namespace Splinter.NanoTypes.Default.Domain.Parameters.Bootstrap;

/// <summary>
/// The extended NanoBootstrapParameters used to bootstrap the Splinter environment.
/// </summary>
public record NanoDefaultBootstrapParameters : NanoBootstrapParameters
{
    /// <summary>
    /// The collection of JSON file names to be used for configuration purposes.
    /// </summary>
    public IEnumerable<string> JsonSettingFileNames { get; init; } = Enumerable.Empty<string>();
}