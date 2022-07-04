using System;

namespace Splinter.NanoTypes.Default.Domain.Settings.Caching;

public class TeraAgentCacheSettings
{
    public TimeSpan SlidingExpirationTimespan { get; set; }
}