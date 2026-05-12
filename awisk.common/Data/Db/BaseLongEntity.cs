using Dapper.Contrib.Extensions;

namespace awisk.common.Data.Db
{
    public abstract class BaseLongEntity
    {
        [Key]
        public long Id { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set; }
        public string DeletedBy { get; set; } = string.Empty;
        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
    }
}
