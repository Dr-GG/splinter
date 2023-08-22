namespace Splinter.NanoTypes.Domain.Exceptions.Services;

/// <summary>
/// The exception that is generated when an invalid operation was performed in a NanoService.
/// </summary>
public class InvalidNanoServiceOperationException : SplinterException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public InvalidNanoServiceOperationException(string message) : base(message)
    { }
}