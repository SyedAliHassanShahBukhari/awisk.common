using awisk.common.Services;
using Microsoft.AspNetCore.Http;

namespace awisk.common.Middleware
{
    public class CorrelationIdMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers[CorrelationIdService.HeaderName].FirstOrDefault()
                ?? Guid.NewGuid().ToString();

            context.Items[CorrelationIdService.HeaderName] = correlationId;
            context.Response.Headers[CorrelationIdService.HeaderName] = correlationId;

            await next(context).ConfigureAwait(false);
        }
    }
}
