using awisk.common.Classes;
using awisk.common.Interfaces;
using awisk.common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace awisk.common.ServiceCollection
{
    public static class FileStorageExtensions
    {
        public static IServiceCollection AddLocalFileStorage(this IServiceCollection services, IConfiguration configuration, string sectionName = "FileStorageSettings")
        {
            services.Configure<FileStorageSettings>(configuration.GetSection(sectionName));
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            return services;
        }

        public static IServiceCollection AddLocalFileStorage(this IServiceCollection services, Action<FileStorageSettings> configure)
        {
            services.Configure(configure);
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            return services;
        }
    }
}
