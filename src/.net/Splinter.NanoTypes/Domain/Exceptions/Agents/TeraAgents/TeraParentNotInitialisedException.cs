namespace Splinter.NanoTypes.Domain.Exceptions.Agents.TeraAgents;

/// <summary>
/// The exception that is generated when the TeraParent of a Nano Type agent has not yet been initialised.
/// </summary>
public class TeraParentNotInitialisedException : SplinterException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public TeraParentNotInitialisedException() : base("The Tera Parent has not yet been initialised.")
    { }
}