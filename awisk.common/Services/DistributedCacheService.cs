using awisk.common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace awisk.common.Services
{
    public class DistributedCacheService(IDistributedCache cache) : ICacheService
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            var bytes = await cache.GetAsync(key, ct).ConfigureAwait(false);
            return bytes is null ? default : JsonSerializer.Deserialize<T>(bytes, _jsonOptions);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
        {
            var options = new DistributedCacheEntryOptions();
            if (expiration.HasValue)
                options.AbsoluteExpirationRelativeToNow = expiration;

            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, _jsonOptions);
            await cache.SetAsync(key, bytes, options, ct).ConfigureAwait(false);
        }

        public async Task RemoveAsync(string key, CancellationToken ct = default) =>
            await cache.RemoveAsync(key, ct).ConfigureAwait(false);

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken ct = default)
        {
            var cached = await GetAsync<T>(key, ct).ConfigureAwait(false);
            if (cached is not null) return cached;

            var value = await factory().ConfigureAwait(false);
            await SetAsync(key, value, expiration, ct).ConfigureAwait(false);
            return value;
        }
    }
}
