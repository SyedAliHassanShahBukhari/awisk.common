using awisk.common.Classes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace awisk.common.ServiceCollection
{
    public static class SwaggerGenWithToken
    {
        public static void InitSwaggerGenWithToken(this IServiceCollection services, SwaggerGen model)
        {
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc(model.Version, new OpenApiInfo { Title = model.Title, Version = model.Version });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
    }
}
