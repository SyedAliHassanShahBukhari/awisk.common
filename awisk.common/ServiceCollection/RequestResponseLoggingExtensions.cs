using awisk.common.Middleware;
using Microsoft.AspNetCore.Builder;

namespace awisk.common.ServiceCollection
{
    public static class RequestResponseLoggingExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
            => app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}
