using Dapper.Contrib.Extensions;

namespace awisk.common.Data.Db
{
    public abstract class BaseIntEntity
    {
        [Key]
        public int Id { get; set; }
        public string CreatedBy { get; set; } = Guid.Empty.ToString();
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; } = Guid.Empty.ToString();
        public DateTime UpdatedOn { get; set; } = new(1900, 01, 01);
        public string DeletedBy { get; set; } = Guid.Empty.ToString();
        public DateTime DeletedOn { get; set; } = new(1900, 01, 01);
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
    }
}
