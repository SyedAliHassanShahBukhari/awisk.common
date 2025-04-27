using awisk.common.Classes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace awisk.common.ServiceCollection
{
    public static partial class DbContextWrapper
    {
        public static void AddSqlDbContextDefault(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void AddSqlDbContextDefault(this IServiceCollection services, string connectionString, string migrationAssembly)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly(migrationAssembly)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void AddSqlDbContextWithPasswordSettings(this IServiceCollection services, ApplicationSettings settings)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(settings.ConnectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = settings.PasswordSettings.RequireDigit;
                options.Password.RequireLowercase = settings.PasswordSettings.RequireLowercase;
                options.Password.RequireUppercase = settings.PasswordSettings.RequireUppercase;
                options.Password.RequireNonAlphanumeric = settings.PasswordSettings.RequireNonAlphanumeric;
                options.Password.RequiredLength = settings.PasswordSettings.RequiredLength;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void AddSqlDbContextWithPasswordSettings(this IServiceCollection services, ApplicationSettings settings, string migrationAssembly)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(settings.ConnectionString, b => b.MigrationsAssembly(migrationAssembly)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = settings.PasswordSettings.RequireDigit;
                options.Password.RequireLowercase = settings.PasswordSettings.RequireLowercase;
                options.Password.RequireUppercase = settings.PasswordSettings.RequireUppercase;
                options.Password.RequireNonAlphanumeric = settings.PasswordSettings.RequireNonAlphanumeric;
                options.Password.RequiredLength = settings.PasswordSettings.RequiredLength;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void AddMySqlDbContextDefault(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void AddMySqlDbContextDefault(this IServiceCollection services, string connectionString, string migrationAssembly)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b => b.MigrationsAssembly(migrationAssembly)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void AddMySqlDbContextWithPasswordSettings(this IServiceCollection services, ApplicationSettings settings)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(settings.ConnectionString, ServerVersion.AutoDetect(settings.ConnectionString)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = settings.PasswordSettings.RequireDigit;
                options.Password.RequireLowercase = settings.PasswordSettings.RequireLowercase;
                options.Password.RequireUppercase = settings.PasswordSettings.RequireUppercase;
                options.Password.RequireNonAlphanumeric = settings.PasswordSettings.RequireNonAlphanumeric;
                options.Password.RequiredLength = settings.PasswordSettings.RequiredLength;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void AddMySqlDbContextWithPasswordSettings(this IServiceCollection services, ApplicationSettings settings, string migrationAssembly)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(settings.ConnectionString, ServerVersion.AutoDetect(settings.ConnectionString), b => b.MigrationsAssembly(migrationAssembly)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = settings.PasswordSettings.RequireDigit;
                options.Password.RequireLowercase = settings.PasswordSettings.RequireLowercase;
                options.Password.RequireUppercase = settings.PasswordSettings.RequireUppercase;
                options.Password.RequireNonAlphanumeric = settings.PasswordSettings.RequireNonAlphanumeric;
                options.Password.RequiredLength = settings.PasswordSettings.RequiredLength;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }
        public static void AddPostgreSqlDbContextDefault(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void AddPostgreSqlDbContextDefault(this IServiceCollection services, string connectionString, string migrationAssembly)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString, b => b.MigrationsAssembly(migrationAssembly)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void AddPostgreSqlDbContextWithPasswordSettings(this IServiceCollection services, ApplicationSettings settings)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(settings.ConnectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = settings.PasswordSettings.RequireDigit;
                options.Password.RequireLowercase = settings.PasswordSettings.RequireLowercase;
                options.Password.RequireUppercase = settings.PasswordSettings.RequireUppercase;
                options.Password.RequireNonAlphanumeric = settings.PasswordSettings.RequireNonAlphanumeric;
                options.Password.RequiredLength = settings.PasswordSettings.RequiredLength;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void AddPostgreSqlDbContextWithPasswordSettings(this IServiceCollection services, ApplicationSettings settings, string migrationAssembly)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(settings.ConnectionString, b => b.MigrationsAssembly(migrationAssembly)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = settings.PasswordSettings.RequireDigit;
                options.Password.RequireLowercase = settings.PasswordSettings.RequireLowercase;
                options.Password.RequireUppercase = settings.PasswordSettings.RequireUppercase;
                options.Password.RequireNonAlphanumeric = settings.PasswordSettings.RequireNonAlphanumeric;
                options.Password.RequiredLength = settings.PasswordSettings.RequiredLength;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }
    }
}
