namespace Splinter.NanoTypes.Domain.Parameters.Termination;

/// <summary>
/// The nano parameters that is used to terminate of a Nano Type.
/// </summary>
public record NanoTerminationParameters : INanoParameters
{
    /// <summary>
    /// The flag indicating if the Nano Type is forced to shutdown.
    /// </summary>
    public bool Force { get; init; }
}