using awisk.common.Interfaces;
using awisk.common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace awisk.common.ServiceCollection
{
    /// <summary>
    /// Extension methods for registering exception logging services.
    /// </summary>
    public static class ExceptionLogServiceExtensions
    {
        /// <summary>
        /// Adds the exception logging service to the service collection.
        /// Requires that IRepositoryBase is already registered in the DI container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddExceptionLogService(this IServiceCollection services)
        {
            services.AddScoped<IExceptionLogService, ExceptionLogService>();
            return services;
        }
    }
}

