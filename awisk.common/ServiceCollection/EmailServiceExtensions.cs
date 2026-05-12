using awisk.common.Classes;
using awisk.common.Interfaces;
using awisk.common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace awisk.common.ServiceCollection
{
    public static class EmailServiceExtensions
    {
        public static IServiceCollection AddSmtpEmailService(this IServiceCollection services, IConfiguration configuration, string sectionName = "EmailSettings")
        {
            services.Configure<EmailSettings>(configuration.GetSection(sectionName));
            services.AddScoped<IEmailService, SmtpEmailService>();
            return services;
        }

        public static IServiceCollection AddSmtpEmailService(this IServiceCollection services, Action<EmailSettings> configure)
        {
            services.Configure(configure);
            services.AddScoped<IEmailService, SmtpEmailService>();
            return services;
        }
    }
}
