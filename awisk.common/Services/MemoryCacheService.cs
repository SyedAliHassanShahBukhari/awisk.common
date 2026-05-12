using awisk.common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace awisk.common.Services
{
    public class MemoryCacheService(IMemoryCache cache) : ICacheService
    {
        public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            cache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
                options.AbsoluteExpirationRelativeToNow = expiration;

            cache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken ct = default)
        {
            cache.Remove(key);
            return Task.CompletedTask;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken ct = default)
        {
            if (cache.TryGetValue(key, out T? cached) && cached is not null)
                return cached;

            var value = await factory().ConfigureAwait(false);
            await SetAsync(key, value, expiration, ct).ConfigureAwait(false);
            return value;
        }
    }
}
