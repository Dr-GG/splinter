using Splinter.NanoTypes.Default.Domain.Settings.Caching;
using Splinter.NanoTypes.Default.Domain.Settings.Messaging;
using Splinter.NanoTypes.Default.Domain.Settings.SplinterIds;
using Splinter.NanoTypes.Default.Domain.Settings.Superposition;

namespace Splinter.NanoTypes.Default.Domain.Settings;

/// <summary>
/// The settings used to setup the default execution environment of a Splinter environment.
/// </summary>
public class SplinterDefaultSettings
{
    /// <summary>
    /// The Splinter superposition settings.
    /// </summary>
    public SuperpositionSettings Superposition { get; set; } = null!;

    /// <summary>
    /// The settings of the INanoTypeCache service.
    /// </summary>
    public NanoTypeCacheSettings NanoTypeCache { get; set; } = null!;

    /// <summary>
    /// The settings of the ITeraTypeCache service.
    /// </summary>
    public TeraAgentCacheSettings TeraAgentCache { get; set; } = null!;

    /// <summary>
    /// The collection of pre-defined Splinter agent id's.
    /// </summary>
    public SplinterTeraAgentIdSettings SplinterTeraAgentIds { get; set; } = null!;

    /// <summary>
    /// The messaging settings used by messaging mechanisms in Splinter.
    /// </summary>
    public TeraMessagingSettings Messaging { get; set; } = null!;
}