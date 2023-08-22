using System;

namespace Splinter.NanoTypes.Default.Domain.Settings.Caching;

/// <summary>
/// The settings of the ITeraAgentCache.
/// </summary>
public class TeraAgentCacheSettings
{
    /// <summary>
    /// The sliding time span indicating when the cache entries of the ITeraAgentCache expires.
    /// </summary>
    public TimeSpan SlidingExpirationTimespan { get; set; }
}