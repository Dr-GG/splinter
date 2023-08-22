namespace Splinter.NanoTypes.Domain.Exceptions.Containers;

/// <summary>
/// The exception that is generated when a ITeraAgentContainer has not yet been initialised.
/// </summary>
public class TeraAgentContainerNotInitialisedException : SplinterException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public TeraAgentContainerNotInitialisedException() : base("The Tera Agent container has not yet been initialised.")
    { }
}