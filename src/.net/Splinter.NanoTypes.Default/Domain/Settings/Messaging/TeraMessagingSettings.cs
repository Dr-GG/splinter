namespace Splinter.NanoTypes.Default.Domain.Settings.Messaging;

/// <summary>
/// The settings used for the general functionality of messaging between Tera Agents.
/// </summary>
public class TeraMessagingSettings
{
    /// <summary>
    /// The maximum number of TeraMessages to be fetched per batch.
    /// </summary>
    public int MaximumTeraAgentFetchCount { get; set; }

    /// <summary>
    /// The maximum number of tries to dequeue and process a TeraMessage.
    /// </summary>
    public int MaximumDequeueRetryCount { get; set; }

    /// <summary>
    /// The maximum number of tries to synchronise the state of a TeraMessage.
    /// </summary>
    public int MaximumSyncRetryCount { get; set; }

    /// <summary>
    /// The settings to be used in handling expired TeraMessage instances.
    /// </summary>
    public ExpiredTeraMessageDisposeSettings ExpiredDisposing { get; set; } = null!;
}