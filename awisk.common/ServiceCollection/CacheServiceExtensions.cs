using awisk.common.Interfaces;
using awisk.common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace awisk.common.ServiceCollection
{
    public static class CacheServiceExtensions
    {
        public static IServiceCollection AddMemoryCacheService(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            return services;
        }

        public static IServiceCollection AddDistributedCacheService(this IServiceCollection services, string redisConnectionString)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });
            services.AddSingleton<ICacheService, DistributedCacheService>();
            return services;
        }
    }
}
