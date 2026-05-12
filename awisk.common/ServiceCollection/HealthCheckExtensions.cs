using Microsoft.Extensions.DependencyInjection;

namespace awisk.common.ServiceCollection
{
    public static class HealthCheckExtensions
    {
        public static IHealthChecksBuilder AddSqlServerHealthCheck(this IHealthChecksBuilder builder, string connectionString, string name = "sqlserver")
            => builder.AddSqlServer(connectionString, name: name);

        public static IHealthChecksBuilder AddMySqlHealthCheck(this IHealthChecksBuilder builder, string connectionString, string name = "mysql")
            => builder.AddMySql(connectionString, name: name);

        public static IHealthChecksBuilder AddPostgreSqlHealthCheck(this IHealthChecksBuilder builder, string connectionString, string name = "postgresql")
            => builder.AddNpgSql(connectionString, name: name);
    }
}
