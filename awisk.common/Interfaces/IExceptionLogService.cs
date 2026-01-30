namespace awisk.common.Interfaces
{
    /// <summary>
    /// Service interface for logging exceptions to the database.
    /// </summary>
    public interface IExceptionLogService
    {
        /// <summary>
        /// Logs an exception to the database asynchronously.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="url">The URL or path where the exception occurred. Optional.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task LogExceptionAsync(Exception exception, string? url = null);

        /// <summary>
        /// Logs an exception message and stack trace to the database asynchronously.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="stackTrace">The stack trace to log. Optional.</param>
        /// <param name="url">The URL or path where the exception occurred. Optional.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task LogExceptionAsync(string message, string? stackTrace = null, string? url = null);
    }
}

