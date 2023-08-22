using System;

namespace Splinter.NanoTypes.Default.Domain.Settings.Caching;

/// <summary>
/// The settings of the INanoTypeCache.
/// </summary>
public class NanoTypeCacheSettings
{
    /// <summary>
    /// The sliding time span indicating when the cache entries of the INanoTypeCache expires.
    /// </summary>
    public TimeSpan SlidingExpirationTimespan { get; set; }
}