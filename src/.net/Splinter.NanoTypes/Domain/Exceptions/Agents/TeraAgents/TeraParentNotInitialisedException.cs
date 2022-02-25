namespace Splinter.NanoTypes.Domain.Exceptions.Agents.TeraAgents
{
    public class TeraParentNotInitialisedException : SplinterException
    {
        public TeraParentNotInitialisedException() : 
            base("The Tera Parent has not yet been initialised")
        { }
    }
}
