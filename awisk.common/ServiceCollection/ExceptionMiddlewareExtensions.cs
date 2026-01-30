using awisk.common.Middleware;
using Microsoft.AspNetCore.Builder;

namespace awisk.common.ServiceCollection
{
    /// <summary>
    /// Extension methods for configuring exception handling middleware.
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Adds the global exception handler middleware to the application pipeline.
        /// This middleware catches unhandled exceptions and returns consistent error responses.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The application builder for chaining.</returns>
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}

