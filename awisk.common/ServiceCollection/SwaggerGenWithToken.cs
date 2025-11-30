using awisk.common.Classes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace awisk.common.ServiceCollection
{
    public static class SwaggerGenWithToken
    {
        /// <summary>
        /// Registers SwaggerGen with JWT Bearer token support, XML comments, and optional contact/license info.
        /// </summary>
        public static void InitSwaggerGenWithToken(this IServiceCollection services, SwaggerGen model)
        {
            services.AddSwaggerGen(option =>
            {
                // Basic info
                option.SwaggerDoc(model.Version, new OpenApiInfo
                {
                    Title = model.Title,
                    Version = model.Version,
                    Description = model.Description ?? $"{model.Title} API Documentation",
                    Contact = new OpenApiContact
                    {
                        Name = model.ContactName ?? "Awisk Team",
                        Email = model.ContactEmail ?? "support@awisk.com",
                        Url = model.ContactUrl != null ? new Uri(model.ContactUrl) : null
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                // Add JWT Security
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token with 'Bearer ' prefix. Example: Bearer 12345abcdef"
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
    }
}
