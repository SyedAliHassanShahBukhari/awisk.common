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
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedOn { get; set; } = new(1900, 01, 01);
        public string DeletedBy { get; set; } = string.Empty;
        public DateTime DeletedOn { get; set; } = new(1900, 01, 01);
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
    }
}
