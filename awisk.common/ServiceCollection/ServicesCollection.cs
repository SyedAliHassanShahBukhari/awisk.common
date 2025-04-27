using awisk.common.Interfaces;
using awisk.common.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace awisk.common.ServiceCollection
{
    public static partial class ServicesCollection
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddHttpClient<IApiService, ApiService>();
            services.AddScoped<ITokenService, TokenService>();
        }
    }
}
