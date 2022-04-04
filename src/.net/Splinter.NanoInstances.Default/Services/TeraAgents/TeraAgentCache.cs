using System;
using Microsoft.Extensions.Caching.Memory;
using Splinter.NanoTypes.Default.Domain.Settings.Caching;
using Splinter.NanoTypes.Default.Interfaces.Services.TeraAgents;

namespace Splinter.NanoInstances.Default.Services.TeraAgents
{
    public class TeraAgentCache : ITeraAgentCache
    {
        private readonly TeraAgentCacheSettings _settings;
        private readonly MemoryCache _cache = new(new MemoryCacheOptions());

        public TeraAgentCache(TeraAgentCacheSettings settings)
        {
            _settings = settings;
        }

        public void RegisterTeraId(Guid teraId, long id)
        {
            var key = GetTeraAgentKey(teraId);

            _cache.Set(key, id, GetDefaultOptions());
        }

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
}
