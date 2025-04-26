using awisk.common.Data.Db;
using awisk.common.Data.Db.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace awisk.common.ServiceCollection
{
    public static partial class DbServiceCollection
    {
        public static void RegisterDbRepos(this IServiceCollection services, IRepositoryBase repositoryBase)
        {
            services.AddSingleton(repositoryBase);
            AddConcreteImplementations(services);
        }
        public static void AddConcreteImplementations(IServiceCollection services)
        {
        }
    }
}
