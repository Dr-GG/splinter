namespace Splinter.NanoTypes.Default.Domain.Settings.Messaging;

public class TeraMessagingSettings
{
    public int MaximumTeraAgentFetchCount { get; set; }
    public int MaximumDequeueRetryCount { get; set; }
    public int MaximumSyncRetryCount { get; set; }
    public TeraMessageDisposeSettings Disposing { get; set; } = null!;
}