using System;

namespace Splinter.NanoTypes.Default.Domain.Settings.Caching;

public class NanoTypeCacheSettings
{
    public TimeSpan SlidingExpirationTimespan { get; set; }
}