namespace Splinter.NanoTypes.Domain.Exceptions.Superposition;

/// <summary>
/// The exception that is generated when a NanoTable has not yet been initialised.
/// </summary>
public class NanoTableNotInitialisedException : SplinterException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoTableNotInitialisedException() : base("The Nano Table has not yet been initialised.")
    { }
}