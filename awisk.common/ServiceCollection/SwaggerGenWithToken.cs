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

                // Include XML Comments (if generated)
                var xmlFilename = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                if (File.Exists(xmlPath))
                    option.IncludeXmlComments(xmlPath);

                // Support Enum Descriptions
                option.SchemaFilter<EnumDescriptionSchemaFilter>();

                // Optional: group controllers by namespace (helpful in large APIs)
                option.TagActionsBy(api =>
                {
                    var controllerName = api.GroupName ?? api.ActionDescriptor?.RouteValues["controller"];
                    return [controllerName ?? "General"];
                });

                option.DocInclusionPredicate((docName, apiDesc) => true);
            });
        }
    }

    // Helper: show enum [Description] attributes nicely in Swagger
    internal class EnumDescriptionSchemaFilter : Swashbuckle.AspNetCore.SwaggerGen.ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                var names = Enum.GetNames(context.Type);
                var values = Enum.GetValues(context.Type).Cast<object>().ToArray();
                var descs = values.Select(v =>
                {
                    var fi = context.Type.GetField(v.ToString()!);
                    var da = fi?.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                                .FirstOrDefault() as System.ComponentModel.DescriptionAttribute;
                    return $"{Convert.ToInt32(v)} = {da?.Description ?? v.ToString()}";
                });
                schema.Description += "<br><b>Enum values:</b><br>" + string.Join("<br>", descs);
            }
        }
    }
}
