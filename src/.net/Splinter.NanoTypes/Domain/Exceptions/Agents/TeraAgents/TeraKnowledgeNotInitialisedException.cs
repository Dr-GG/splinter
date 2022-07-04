namespace Splinter.NanoTypes.Domain.Exceptions.Agents.TeraAgents;

public class TeraKnowledgeNotInitialisedException : SplinterException
{
    public TeraKnowledgeNotInitialisedException() : 
        base("The Tera Knowledge has not yet been initialised")
    { }
}