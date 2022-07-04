namespace Splinter.NanoTypes.Domain.Exceptions.Superposition;

public class NanoTableNotInitialisedException : SplinterException
{
    public NanoTableNotInitialisedException() : 
        base("The Nano Table has not been initialised")
    { }
}