using awisk.common.Classes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace awisk.common.ServiceCollection
{
    public static partial class DbContextWrapper
    {
        public static void AddSqlDbContextDefault<T>(this IServiceCollection services, string connectionString) where T : DbContext
        {
            services.AddDbContext<T>(options =>
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

        public static void AddSqlDbContextDefault<T>(this IServiceCollection services, string connectionString, string migrationAssembly) where T : DbContext
        {
            services.AddDbContext<T>(options =>
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

        public static void AddSqlDbContextDefault<TContext, TUser>(this IServiceCollection services, string connectionString, string migrationAssembly)
            where TContext : IdentityDbContext<TUser>
            where TUser : IdentityUser
        {
            services.AddDbContext<TContext>(options =>
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly(migrationAssembly)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<TUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<TContext>()
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

        public static void AddMySqlDbContextDefault<T>(this IServiceCollection services, string connectionString) where T : DbContext
        {
            services.AddDbContext<T>(options =>
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

        public static void AddMySqlDbContextDefault<T>(this IServiceCollection services, string connectionString, string migrationAssembly) where T : DbContext
        {
            services.AddDbContext<T>(options =>
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

        public static void AddMySqlDbContextDefault<TContext, TUser>(this IServiceCollection services, string connectionString, string migrationAssembly)
            where TContext : IdentityDbContext<TUser>
            where TUser : IdentityUser
        {
            services.AddDbContext<TContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b => b.MigrationsAssembly(migrationAssembly)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<TUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<TContext>()
            .AddDefaultTokenProviders();
        }

        // Non-generic overloads for backward compatibility
        public static void AddMySqlDbContextDefault(this IServiceCollection services, string connectionString)
        {
            services.AddMySqlDbContextDefault<ApplicationDbContext>(connectionString);
        }

        public static void AddMySqlDbContextDefault(this IServiceCollection services, string connectionString, string migrationAssembly)
        {
            services.AddMySqlDbContextDefault<ApplicationDbContext>(connectionString, migrationAssembly);
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
        public static void AddPostgreSqlDbContextDefault<T>(this IServiceCollection services, string connectionString) where T : DbContext
        {
            services.AddDbContext<T>(options =>
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

        public static void AddPostgreSqlDbContextDefault<T>(this IServiceCollection services, string connectionString, string migrationAssembly) where T : DbContext
        {
            services.AddDbContext<T>(options =>
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

        public static void AddPostgreSqlDbContextDefault<TContext, TUser>(this IServiceCollection services, string connectionString, string migrationAssembly)
            where TContext : IdentityDbContext<TUser>
            where TUser : IdentityUser
        {
            services.AddDbContext<TContext>(options =>
                options.UseNpgsql(connectionString, b => b.MigrationsAssembly(migrationAssembly)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<TUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<TContext>()
            .AddDefaultTokenProviders();
        }

        // Non-generic overloads for backward compatibility
        public static void AddPostgreSqlDbContextDefault(this IServiceCollection services, string connectionString)
        {
            services.AddPostgreSqlDbContextDefault<ApplicationDbContext>(connectionString);
        }

        public static void AddPostgreSqlDbContextDefault(this IServiceCollection services, string connectionString, string migrationAssembly)
        {
            services.AddPostgreSqlDbContextDefault<ApplicationDbContext>(connectionString, migrationAssembly);
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
