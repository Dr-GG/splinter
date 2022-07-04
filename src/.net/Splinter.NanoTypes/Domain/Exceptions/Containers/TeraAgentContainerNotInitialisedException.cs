namespace Splinter.NanoTypes.Domain.Exceptions.Containers;

public class TeraAgentContainerNotInitialisedException : SplinterException
{
    public TeraAgentContainerNotInitialisedException() :
        base("The Tera Agent container has not yet been initialised")
    { }
}