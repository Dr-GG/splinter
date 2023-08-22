namespace Splinter.NanoTypes.Domain.Exceptions.NanoWaveFunctions;

/// <summary>
/// The exception that is generated when an invalid Nano Type was collapsed or retrieved.
/// </summary>
public class InvalidNanoTypeException : SplinterException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public InvalidNanoTypeException(string message) : base(message)
    { }
}