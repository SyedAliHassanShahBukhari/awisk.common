using FluentMigrator.Runner;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace awisk.common.Migrators
{
    public partial class SqlMigrator
    {
        private static string MIGRATIONS_NAMESPACE = "";
        private static string APP_NAME = "";
        public static void Migrator(string[] args, string migrationNameSpace, string appName)
        {
            MIGRATIONS_NAMESPACE = migrationNameSpace;
            APP_NAME = appName;
            string connectionStringSqlAuth = "Data Source={0};Initial Catalog={1};user id={2};password={3};TrustServerCertificate=True";
            string connectionStringWindowsAuth = "Data Source={0};Initial Catalog={1};Integrated Security=SSPI;TrustServerCertificate=True";
            string connectionString = "";
            var app = new CommandLineApplication
            {
                Name = APP_NAME
            };
            var server = app.Argument("server", "the server name where the database is hosted");
            var database = app.Argument("database", "the name of the database to migrate");
            var user = app.Option("-u|--username", "the username to use to authenticate to the database", CommandOptionType.SingleValue);
            var pwd = app.Option("-p|--password", "the password to use to authenticate to the database", CommandOptionType.SingleValue);
            var migrate = app.Option("-m|--migrate", "the direction to migrate, valid values: up, down", CommandOptionType.SingleValue);
            var version = app.Option("-v|--version", "the version number to migrate to", CommandOptionType.SingleValue);
            var quiet = app.Option("-q|--quiet", "will run with out confirming the server and database to migrate", CommandOptionType.NoValue);
            app.HelpOption("-?|-h|--help");
            app.OnExecute(() =>
            {
                Console.WriteLine($"server = {server.Value} ");
                Console.WriteLine($"db = {database.Value}");
                Console.WriteLine($"quiet? = {quiet.HasValue()}");
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
                    var runMigrations = quiet.HasValue();

                    if (!quiet.HasValue())
                    {
                        Console.Write("Please confirm you would like to run the migrations (Y/N)");

                        var key = Console.ReadKey();
                        Console.WriteLine("");
                        if (key.Key == ConsoleKey.Y)
                        {
                            runMigrations = true;
                        }
                        else
                        {
                            runMigrations = false;
                            Console.WriteLine("ABORTING");
                        }
                    }

                    if (runMigrations)
                    {
                        if (String.IsNullOrEmpty(user.Value()) || String.IsNullOrEmpty(pwd.Value()))
                        {
                            connectionString = string.Format(connectionStringWindowsAuth, server.Value, database.Value);
                        }
                        else
                        {
                            connectionString = string.Format(connectionStringSqlAuth, server.Value, database.Value, user.Value(), pwd.Value());
                        }

                        var serviceProvider = CreateServices(connectionString);

                        // Put the database update into a scope to ensure
                        // that all resources will be disposed.
                        using (var scope = serviceProvider.CreateScope())
                        {
                            switch (migrate.Value())
                            {
                                case "down":
                                    DowngradeDatabase(scope.ServiceProvider, Convert.ToInt64(version.Value()));
                                    break;
                                default:
                                    UpgradeDatabase(scope.ServiceProvider);
                                    break;
                            }
                        }

                        Console.WriteLine("All Migration Complete");
                    }
                    else
                    {
                        Console.Error.WriteLine("NO MIGRATIONS RAN");
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Migration failed: {ex.Message}");
                    Console.Error.WriteLine($"Stack Trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        Console.Error.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                        Console.Error.WriteLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
                    }
                    throw; // Re-throw to indicate failure
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
                // Add common FluentMigrator services

                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add SQLite support to FluentMigrator
                    .AddSqlServer()
                    // Set the connection string
                    .WithGlobalConnectionString(connectionString)
                    // Define the assembly containing the migrations
                    //.ScanIn(typeof(AddLogTable).Assembly).For.Migrations())
                    .ScanIn(assemblies).For.Migrations().For.EmbeddedResources()
                    .ScanIn(System.Reflection.Assembly.GetExecutingAssembly()).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
        }

        /// <summary>
        /// Upgrade the database
        /// </summary>
        private static void UpgradeDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateUp();
        }

        /// <summary>
        /// Upgrade the database
        /// </summary>
        private static void DowngradeDatabase(IServiceProvider serviceProvider, long version)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateDown(version);
        }
    }
}
