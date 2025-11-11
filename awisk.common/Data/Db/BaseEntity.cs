using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace awisk.common.Data.Db
{
    public abstract class BaseEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CreatedBy { get; set; } = Guid.Empty.ToString();
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; } = Guid.Empty.ToString();
        public DateTime UpdatedOn { get; set; } = new(1900, 01, 01);
        public string DeletedBy { get; set; } = Guid.Empty.ToString();
        public DateTime DeletedOn { get; set; } = new(1900, 01, 01);
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
    }

    public abstract class BaseEntity<T>
    {
        public T Id { get; set; } = default!;
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
