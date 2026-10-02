namespace awisk.common.Data.Db
{
    /// <summary>
    /// Generic base entity that supports various ID types (int, long, Guid, string, etc.)
    /// Use [Key] attribute for auto-increment IDs (int, long) or [ExplicitKey] for non-auto-increment IDs (Guid, string)
    /// </summary>
    /// <typeparam name="T">The type of the ID property. Common types: int, long, Guid, string</typeparam>
    public abstract class BaseEntity<T>
    {
        /// <summary>
        /// Primary key. Use [Key] for auto-increment or [ExplicitKey] for non-auto-increment IDs.
        /// </summary>
        public T Id { get; set; } = default!;
        public string CreatedBy { get; set; } = "System";
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        /// <summary>NULL until the row is genuinely updated. Never give this an initialiser — a non-null default writes a placeholder on every insert.</summary>
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        /// <summary>NULL until the row is genuinely soft-deleted. Never give this an initialiser.</summary>
        public string? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
    }
}
