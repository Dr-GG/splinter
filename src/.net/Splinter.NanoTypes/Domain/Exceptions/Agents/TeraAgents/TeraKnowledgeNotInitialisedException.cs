namespace Splinter.NanoTypes.Domain.Exceptions.Agents.TeraAgents;

/// <summary>
/// The exception that is generate when a TeraKnowledgeAgent was not successfully initialised.
/// </summary>
public class TeraKnowledgeNotInitialisedException : SplinterException
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public TeraKnowledgeNotInitialisedException() : base("The Tera Knowledge has not yet been initialised.")
    { }
}