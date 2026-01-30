using awisk.common.Classes;
using awisk.common.Data.Db.Interfaces;
using awisk.common.Interfaces;

namespace awisk.common.Services
{
    /// <summary>
    /// Service implementation for logging exceptions to the database.
    /// Uses the repository pattern to store exception logs.
    /// </summary>
    public class ExceptionLogService : IExceptionLogService
    {
        private readonly IRepositoryBase _repository;

        /// <summary>
        /// Initializes a new instance of the ExceptionLogService.
        /// </summary>
        /// <param name="repository">The repository used to persist exception logs.</param>
        public ExceptionLogService(IRepositoryBase repository)
        {
            ArgumentNullException.ThrowIfNull(repository);
            _repository = repository;
        }

        /// <summary>
        /// Logs an exception to the database asynchronously.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="url">The URL or path where the exception occurred. Optional.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task LogExceptionAsync(Exception exception, string? url = null)
        {
            ArgumentNullException.ThrowIfNull(exception);

            var log = new ExceptionLog
            {
                Message = exception.Message ?? string.Empty,
                StackTrace = exception.StackTrace ?? string.Empty,
                Type = exception.GetType().FullName ?? exception.GetType().Name,
                URL = url?.Length > 100 ? url[..100] : (url ?? string.Empty),
                CreatedOn = DateTime.UtcNow
            };

            await _repository.InsertAsync(log).ConfigureAwait(false);
        }

        /// <summary>
        /// Logs an exception message and stack trace to the database asynchronously.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="stackTrace">The stack trace to log. Optional.</param>
        /// <param name="url">The URL or path where the exception occurred. Optional.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task LogExceptionAsync(string message, string? stackTrace = null, string? url = null)
        {
            ArgumentNullException.ThrowIfNull(message);

            var log = new ExceptionLog
            {
                Message = message ?? string.Empty,
                StackTrace = stackTrace ?? string.Empty,
                Type = "ManualLog",
                URL = url?.Length > 100 ? url[..100] : (url ?? string.Empty),
                CreatedOn = DateTime.UtcNow
            };

            await _repository.InsertAsync(log).ConfigureAwait(false);
        }
    }
}

