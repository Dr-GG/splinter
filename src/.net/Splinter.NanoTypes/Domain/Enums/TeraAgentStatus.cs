namespace Splinter.NanoTypes.Domain.Enums;

/// <summary>
/// Depicts the status of a Tera Agent instance.
/// </summary>
public enum TeraAgentStatus
{
    /// <summary>
    /// The Tera Agent is currently alive and running.
    /// </summary>
    Running = 1,

    /// <summary>
    /// The Tera Agent is currently migrating to another platform.
    /// </summary>
    Migrating = 2,

    /// <summary>
    /// The Tera Agent is currently disposed and inactive.
    /// </summary>
    Disposed = 3
}