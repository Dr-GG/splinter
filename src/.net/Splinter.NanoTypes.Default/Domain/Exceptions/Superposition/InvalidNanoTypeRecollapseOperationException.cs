using Splinter.NanoTypes.Domain.Exceptions;

namespace Splinter.NanoTypes.Default.Domain.Exceptions.Superposition;

public class InvalidNanoTypeRecollapseOperationException : SplinterException
{
    public InvalidNanoTypeRecollapseOperationException(string message) : base(message)
    { }
}