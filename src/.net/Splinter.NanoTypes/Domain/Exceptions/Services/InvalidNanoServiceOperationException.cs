namespace Splinter.NanoTypes.Domain.Exceptions.Services;

public class InvalidNanoServiceOperationException : SplinterException
{
    public InvalidNanoServiceOperationException(string message) : base(message)
    { }
}