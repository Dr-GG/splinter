namespace Splinter.NanoTypes.Domain.Exceptions.NanoParameters;

/// <summary>
/// The exception that is generated when invalid INanoParameters were specified.
/// </summary>
public class InvalidNanoParametersException : SplinterException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public InvalidNanoParametersException(string message) : base(message)
    { }
}