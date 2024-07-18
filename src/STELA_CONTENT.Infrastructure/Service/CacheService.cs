using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using STELA_CONTENT.Core.IService;

namespace STELA_CONTENT.Infrastructure.Service
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task SetCache<T>(string key, T value, TimeSpan absoluteExpiration, TimeSpan slidingExpiration)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.Add(absoluteExpiration),
                SlidingExpiration = slidingExpiration
            };
            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
        }

        public async Task<T?> GetCache<T>(string key)
        {
            var value = await _distributedCache.GetStringAsync(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        public async Task RemoveCache(string key) => await _distributedCache.RemoveAsync(key);
    }
}