using awisk.common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace awisk.common.Middleware
{
    public class RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            var correlationId = context.Items[Services.CorrelationIdService.HeaderName]?.ToString() ?? "-";

            logger.LogInformation(
                "HTTP {Method} {Path}{Query} started [CorrelationId: {CorrelationId}]",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                correlationId);

            try
            {
                await next(context).ConfigureAwait(false);
            }
            finally
            {
                sw.Stop();
                logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms [CorrelationId: {CorrelationId}]",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    sw.ElapsedMilliseconds,
                    correlationId);
            }
        }
    }
}
