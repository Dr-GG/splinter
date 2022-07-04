using Splinter.NanoTypes.Default.Domain.Settings.Caching;
using Splinter.NanoTypes.Default.Domain.Settings.Messaging;
using Splinter.NanoTypes.Default.Domain.Settings.SplinterIds;
using Splinter.NanoTypes.Default.Domain.Settings.Superposition;

namespace Splinter.NanoTypes.Default.Domain.Settings;

public class SplinterDefaultSettings
{
    public SuperpositionSettings Superposition { get; set; } = null!;
    public NanoTypeCacheSettings NanoTypeCache { get; set; } = null!;
    public TeraAgentCacheSettings TeraAgentCache { get; set; } = null!;
    public SplinterTeraAgentIdSettings SplinterTeraAgentIds { get; set; } = null!;
    public TeraMessagingSettings Messaging { get; set; } = null!;
}