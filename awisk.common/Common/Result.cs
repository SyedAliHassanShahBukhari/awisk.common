namespace awisk.common.Common
{
    /// <summary>
    /// Represents the result of an operation that can either succeed or fail.
    /// Provides a functional approach to error handling without exceptions.
    /// </summary>
    /// <typeparam name="T">The type of the value returned on success.</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// The value returned on success. Null if the operation failed.
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// The error message if the operation failed. Null if successful.
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// The exception that caused the failure, if any. Null if successful or if no exception was captured.
        /// </summary>
        public Exception? Exception { get; }

        protected Result(bool isSuccess, T? value, string? errorMessage, Exception? exception)
        {
            IsSuccess = isSuccess;
            Value = value;
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        /// <summary>
        /// Creates a successful result with a value.
        /// </summary>
        /// <param name="value">The value to return.</param>
        /// <returns>A successful result containing the value.</returns>
        public static Result<T> Success(T value) => new(true, value, null, null);

        /// <summary>
        /// Creates a failed result with an error message.
        /// </summary>
        /// <param name="errorMessage">The error message describing the failure.</param>
        /// <returns>A failed result with the error message.</returns>
        public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage, null);

        /// <summary>
        /// Creates a failed result from an exception.
        /// </summary>
        /// <param name="exception">The exception that caused the failure.</param>
        /// <returns>A failed result with the exception's message and the exception itself.</returns>
        public static Result<T> Failure(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);
            return new(false, default, exception.Message, exception);
        }

        /// <summary>
        /// Creates a failed result with both an error message and an exception.
        /// </summary>
        /// <param name="errorMessage">The error message describing the failure.</param>
        /// <param name="exception">The exception that caused the failure.</param>
        /// <returns>A failed result with both the error message and exception.</returns>
        public static Result<T> Failure(string errorMessage, Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);
            return new(false, default, errorMessage, exception);
        }

        /// <summary>
        /// Implicitly converts a value to a successful result.
        /// </summary>
        public static implicit operator Result<T>(T value) => Success(value);

        /// <summary>
        /// Implicitly converts a string error message to a failed result.
        /// </summary>
        public static implicit operator Result<T>(string errorMessage) => Failure(errorMessage);

        /// <summary>
        /// Transforms the success value. Passes failure through unchanged.
        /// </summary>
        public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            if (IsSuccess) return Result<TNew>.Success(mapper(Value!));
            return Exception != null
                ? Result<TNew>.Failure(ErrorMessage!, Exception)
                : Result<TNew>.Failure(ErrorMessage!);
        }

        /// <summary>
        /// Chains to another Result-returning operation. Short-circuits on failure.
        /// </summary>
        public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder)
        {
            ArgumentNullException.ThrowIfNull(binder);
            if (IsSuccess) return binder(Value!);
            return Exception != null
                ? Result<TNew>.Failure(ErrorMessage!, Exception)
                : Result<TNew>.Failure(ErrorMessage!);
        }

        /// <summary>
        /// Collapses the result into a single value — no more if/else at call sites.
        /// </summary>
        public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<string, TOut> onFailure)
        {
            ArgumentNullException.ThrowIfNull(onSuccess);
            ArgumentNullException.ThrowIfNull(onFailure);
            return IsSuccess ? onSuccess(Value!) : onFailure(ErrorMessage ?? string.Empty);
        }
    }

    /// <summary>
    /// Represents the result of an operation that doesn't return a value (void operations).
    /// </summary>
    public class Result : Result<object>
    {
        private Result(bool isSuccess, object? value, string? errorMessage, Exception? exception)
            : base(isSuccess, value, errorMessage, exception)
        {
        }

        /// <summary>
        /// Creates a successful result for a void operation.
        /// </summary>
        /// <returns>A successful result.</returns>
        public static new Result Success() => new(true, null, null, null);

        /// <summary>
        /// Creates a failed result with an error message.
        /// </summary>
        /// <param name="errorMessage">The error message describing the failure.</param>
        /// <returns>A failed result with the error message.</returns>
        public static new Result Failure(string errorMessage) => new(false, null, errorMessage, null);

        /// <summary>
        /// Creates a failed result from an exception.
        /// </summary>
        /// <param name="exception">The exception that caused the failure.</param>
        /// <returns>A failed result with the exception's message and the exception itself.</returns>
        public static new Result Failure(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);
            return new(false, null, exception.Message, exception);
        }

        /// <summary>
        /// Creates a failed result with both an error message and an exception.
        /// </summary>
        /// <param name="errorMessage">The error message describing the failure.</param>
        /// <param name="exception">The exception that caused the failure.</param>
        /// <returns>A failed result with both the error message and exception.</returns>
        public static new Result Failure(string errorMessage, Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);
            return new(false, null, errorMessage, exception);
        }

        /// <summary>
        /// Implicitly converts a string error message to a failed result.
        /// </summary>
        public static implicit operator Result(string errorMessage) => Failure(errorMessage);
    }
}

