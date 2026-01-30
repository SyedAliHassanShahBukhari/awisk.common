using Dapper.Contrib.Extensions;

namespace awisk.common.Classes
{
    /// <summary>
    /// Represents an exception log entry stored in the database.
    /// Corresponds to the ExceptionLogs table created by ExceptionLogMigration.
    /// </summary>
    [Table("ExceptionLogs")]
    public class ExceptionLog
    {
        /// <summary>
        /// Primary key identifier for the log entry.
        /// </summary>
        [Key]
        public long LogId { get; set; }

        /// <summary>
        /// The exception message or error message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The stack trace of the exception.
        /// </summary>
        public string StackTrace { get; set; } = string.Empty;

        /// <summary>
        /// The type name of the exception.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// The URL or path where the exception occurred (up to 100 characters).
        /// </summary>
        public string URL { get; set; } = string.Empty;

        /// <summary>
        /// The UTC timestamp when the exception was logged.
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}

