using System;
using Microsoft.Extensions.Caching.Memory;
using Splinter.NanoTypes.Default.Domain.Settings.Caching;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents;

namespace Splinter.NanoInstances.Default.Services.TeraAgents;

/// <summary>
/// The default implementation of the ITeraAgentCache interface.
/// </summary>
public class TeraAgentCache : ITeraAgentCache
{
    private readonly TeraAgentCacheSettings _settings;
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public TeraAgentCache(TeraAgentCacheSettings settings)
    {
        _settings = settings;
    }

    /// <inheritdoc />
    public void RegisterTeraId(Guid teraId, long id)
    {
        var key = GetTeraAgentKey(teraId);

        _cache.Set(key, id, GetDefaultOptions());
    }

    /// <inheritdoc />
    public bool TryGetTeraId(Guid teraId, out long id)
    {
        var key = GetTeraAgentKey(teraId);

        return _cache.TryGetValue(key, out id);
    }

    private static string GetTeraAgentKey(Guid teraId)
    {
        return $"tera-id-{teraId}";
    }

    private MemoryCacheEntryOptions GetDefaultOptions()
    {
        return new MemoryCacheEntryOptions
        {
            SlidingExpiration = _settings.SlidingExpirationTimespan,
            Priority = CacheItemPriority.Normal
        };
    }
}