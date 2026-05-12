using awisk.common.DTOs.Responses;
using awisk.common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace awisk.common.Middleware
{
    /// <summary>
    /// Middleware for global exception handling in ASP.NET Core applications.
    /// Catches unhandled exceptions and returns consistent error responses.
    /// Optionally logs exceptions to the database if IExceptionLogService is registered.
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware>? _logger;
        private readonly IExceptionLogService? _exceptionLogService;

        /// <summary>
        /// Initializes a new instance of the GlobalExceptionHandlerMiddleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">Optional logger for exception logging.</param>
        /// <param name="exceptionLogService">Optional exception logging service for database logging.</param>
        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware>? logger = null,
            IExceptionLogService? exceptionLogService = null)
        {
            _next = next;
            _logger = logger;
            _exceptionLogService = exceptionLogService;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Handles exceptions by logging and returning appropriate HTTP responses.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="exception">The exception to handle.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                ArgumentException or ArgumentNullException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                KeyNotFoundException or FileNotFoundException => HttpStatusCode.NotFound,
                NotImplementedException => HttpStatusCode.NotImplemented,
                TimeoutException => HttpStatusCode.RequestTimeout,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new GenericResponseDto<object>
            {
                StatusCode = statusCode,
                Message = exception.Message,
                Response = null
            };

            // Log to console/application logs
            _logger?.LogError(
                exception,
                "Unhandled exception occurred. Path: {Path}, StatusCode: {StatusCode}",
                context.Request.Path,
                statusCode);

            // Log to database if service is available
            if (_exceptionLogService != null)
            {
                try
                {
                    var url = $"{context.Request.Method} {context.Request.Path}{context.Request.QueryString}";
                    await _exceptionLogService.LogExceptionAsync(exception, url).ConfigureAwait(false);
                }
                catch (Exception logException)
                {
                    // Don't fail the request if logging fails
                    _logger?.LogWarning(logException, "Failed to log exception to database");
                }
            }

            var json = JsonSerializer.Serialize(response, _jsonOptions);
            await context.Response.WriteAsync(json).ConfigureAwait(false);
        }
    }
}

