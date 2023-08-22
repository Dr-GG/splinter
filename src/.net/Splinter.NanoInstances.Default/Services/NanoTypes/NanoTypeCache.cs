﻿using Microsoft.Extensions.Caching.Memory;
using Splinter.NanoTypes.Default.Domain.Settings.Caching;
using Splinter.NanoTypes.Default.Interfaces.Services.NanoTypes;
using Splinter.NanoTypes.Domain.Core;

namespace Splinter.NanoInstances.Default.Services.NanoTypes;

/// <summary>
/// The default implementation of the INanoTypeCache interface.
/// </summary>
public class NanoTypeCache : INanoTypeCache
{
    private readonly NanoTypeCacheSettings _settings;
    private readonly MemoryCache _cache = new (new MemoryCacheOptions());

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public NanoTypeCache(NanoTypeCacheSettings settings)
    {
        _settings = settings;
    }

    /// <inheritdoc />
    public void RegisterNanoTypeId(SplinterId nanoType, long id)
    {
        var key = GetNanoTypeIdCacheKey(nanoType);

        _cache.Set(key, id, GetDefaultOptions());
    }

    /// <inheritdoc />
    public void RegisterNanoInstanceId(SplinterId nanoInstance, long id)
    {
        var key = GetNanoInstanceIdCacheKey(nanoInstance);

        _cache.Set(key, id, GetDefaultOptions());
    }

    /// <inheritdoc />
    public bool TryGetNanoTypeId(SplinterId nanoType, out long id)
    {
        var key = GetNanoTypeIdCacheKey(nanoType);

        return _cache.TryGetValue(key, out id);
    }

    /// <inheritdoc />
    public bool TryGetNanoInstanceId(SplinterId nanoInstance, out long id)
    {
        var key = GetNanoInstanceIdCacheKey(nanoInstance);

        return _cache.TryGetValue(key, out id);
    }

    private MemoryCacheEntryOptions GetDefaultOptions()
    {
        return new MemoryCacheEntryOptions
        {
            SlidingExpiration = _settings.SlidingExpirationTimespan,
            Priority = CacheItemPriority.Normal
        };
    }

    private static string GetNanoTypeIdCacheKey(SplinterId id)
    {
        return $"NTID_{id.Guid}_{id.Version}";
    }

    private static string GetNanoInstanceIdCacheKey(SplinterId id)
    {
        return $"NIID_{id.Guid}_{id.Version}";
    }
}