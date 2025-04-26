using FluentMigrator.Runner;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace awisk.common.Migrators
{
    public class PostgreSqlMigrator
    {
        private static string MIGRATIONS_NAMESPACE = "";
        private static string APP_NAME = "";

        public static void Migrator(string[] args, string migrationNamespace, string appName)
        {
            MIGRATIONS_NAMESPACE = migrationNamespace;
            APP_NAME = appName;
            string connectionStringTemplate = "Host={0};Database={1};Username={2};Password={3};";

            var app = new CommandLineApplication
            {
                Name = APP_NAME
            };

            var server = app.Argument("server", "The server name where the database is hosted");
            var database = app.Argument("database", "The name of the database to migrate");
            var user = app.Option("-u|--username", "The username to authenticate", CommandOptionType.SingleValue);
            var pwd = app.Option("-p|--password", "The password to authenticate", CommandOptionType.SingleValue);
            var migrate = app.Option("-m|--migrate", "Migration direction: up or down", CommandOptionType.SingleValue);
            var version = app.Option("-v|--version", "Version to migrate to (used with down)", CommandOptionType.SingleValue);
            var quiet = app.Option("-q|--quiet", "Run without confirmation", CommandOptionType.NoValue);
            app.HelpOption("-?|-h|--help");

            app.OnExecute(() =>
            {
                if (server.Value == null || database.Value == null)
                {
                    app.ShowHelp();
                    return 1;
                }
                return 0;
            });

            int result = app.Execute(args);

            if (result == 0)
            {
                try
                {
                    bool runMigrations = quiet.HasValue();

                    if (!quiet.HasValue())
                    {
                        Console.Write("Confirm running migrations (Y/N): ");
                        var key = Console.ReadKey();
                        Console.WriteLine();
                        runMigrations = key.Key == ConsoleKey.Y;
                        if (!runMigrations)
                        {
                            Console.WriteLine("ABORTING");
                            return;
                        }
                    }

                    string connectionString = string.Format(connectionStringTemplate, server.Value, database.Value, user.Value(), pwd.Value());

                    var serviceProvider = CreateServices(connectionString);

                    using var scope = serviceProvider.CreateScope();
                    if (migrate.Value() == "down" && version.HasValue())
                    {
                        DowngradeDatabase(scope.ServiceProvider, Convert.ToInt64(version.Value()));
                    }
                    else
                    {
                        UpgradeDatabase(scope.ServiceProvider);
                    }

                    Console.WriteLine("Migrations Complete");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Migration failed: {ex.Message}");
                }
            }
            else
            {
                Console.Error.WriteLine("NO MIGRATIONS RAN");
            }
        }

        private static ServiceProvider CreateServices(string connectionString)
        {
            AppDomain.CurrentDomain.Load(MIGRATIONS_NAMESPACE);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).ToArray();

            return new Microsoft.Extensions.DependencyInjection.ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()  // Switch to PostgreSQL provider
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(assemblies).For.Migrations().For.EmbeddedResources()
                    .ScanIn(System.Reflection.Assembly.GetExecutingAssembly()).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        private static void UpgradeDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        private static void DowngradeDatabase(IServiceProvider serviceProvider, long version)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateDown(version);
        }
    }
}
