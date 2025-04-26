using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace awisk.common.Data.Db.Interfaces
{
    public interface IRepositorySettings
    {
        string ConnectionString { get; set; }
    }
}
