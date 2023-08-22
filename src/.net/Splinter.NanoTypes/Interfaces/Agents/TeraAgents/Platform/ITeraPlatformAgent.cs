namespace Splinter.NanoTypes.Interfaces.Agents.TeraAgents.Platform;

/// <summary>
/// The ITeraAgent that reflects a collection of executing ITeraAgent instances.
/// </summary>
public interface ITeraPlatformAgent : ITeraAgent
{
    /// <summary>
    /// The unique ID of the platform.
    /// </summary>
    long TeraPlatformId { get; }
}