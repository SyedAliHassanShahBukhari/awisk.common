using awisk.common.Data.Db.Interfaces;
using Microsoft.Extensions.DependencyInjection;

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
