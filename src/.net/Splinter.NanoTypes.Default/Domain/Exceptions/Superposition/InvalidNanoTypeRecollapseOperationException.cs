using Splinter.NanoTypes.Domain.Exceptions;

namespace Splinter.NanoTypes.Default.Domain.Exceptions.Superposition;

/// <summary>
/// The exception that is generated when an invalid Nano Type was provided for recollapsing purposes.
/// </summary>
public class InvalidNanoTypeRecollapseOperationException : SplinterException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public InvalidNanoTypeRecollapseOperationException(string message) : base(message)
    { }
}