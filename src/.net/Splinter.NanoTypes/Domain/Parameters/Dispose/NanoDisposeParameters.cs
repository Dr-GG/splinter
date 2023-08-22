namespace Splinter.NanoTypes.Domain.Parameters.Dispose;

/// <summary>
/// The nano parameters that is used to dispose of a Nano Type.
/// </summary>
public record NanoDisposeParameters : NanoParameters
{
    /// <summary>
    /// The flag indicating if the Nano Type is forced to shutdown.
    /// </summary>
    public bool Force { get; init; }
}