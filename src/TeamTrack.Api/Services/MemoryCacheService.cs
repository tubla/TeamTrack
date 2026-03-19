using Microsoft.Extensions.Caching.Memory;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Services
{
    public class MemoryCacheService(IMemoryCache cache) : ICacheService
    {
        private readonly IMemoryCache _cache = cache;

        public Task<T?> GetAsync<T>(string key)
        {
            _cache.TryGetValue(key, out T value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
            };

            _cache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}