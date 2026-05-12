using awisk.common.Interfaces;
using awisk.common.Middleware;
using awisk.common.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace awisk.common.ServiceCollection
{
    public static class CorrelationIdExtensions
    {
        public static IServiceCollection AddCorrelationId(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICorrelationIdService, CorrelationIdService>();
            return services;
        }

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
            => app.UseMiddleware<CorrelationIdMiddleware>();
    }
}
