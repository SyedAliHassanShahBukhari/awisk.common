using Dapper.Contrib.Extensions;

namespace awisk.common.Data.Db
{
    /// <summary>
    /// Base entity class with Guid as the primary key type.
    /// Use this for entities that require globally unique identifiers.
    /// </summary>
    public abstract class BaseGuidEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; } = Guid.NewGuid();
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
