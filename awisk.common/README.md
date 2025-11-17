# awisk.common

## Compatible with .NET 9 (net9.0)

`awisk.Common` is a shared utility library that provides reusable components, helpers, and extensions commonly used across .NET projects. This package helps ensure consistency, maintainability, and reduces boilerplate code in applications targeting .NET 9.

---

## Features

- Authentication and JWT support
  - `TokenService`, `JwtSettings`, `AuthConfig`, `AddAuthenticationWrapper`
- API helpers
  - `ApiService`, `IApiService`
- DTOs and responses
  - `TokenResponseDto`, `ListItemResponseDto`, `GenericResponseDto`
- Database repositories and migrations
  - `RepositoryBasePostgreSql`, `RepositoryBaseMySql`, `RepositoryBaseSqlServer`
  - `PostgreSqlMigrator`, `MySqlMigrator`, `SqlMigrator`
  - `ExceptionLogMigration`, migration helpers and `DataTypes`
- Helpers and extensions
  - `UniversalOpertaions`, `EnumHelper`, `RandomCodeGenerator`, `MigrationExtensions`
- Swagger with bearer token support
  - `SwaggerGenWithToken`
- Service registration helpers for DI
  - `DbServiceCollection`, `SwaggerGenWithToken`, authentication/service wrappers
- Config and settings classes
  - `ApplicationSettings`, `ApiSettings`, `PasswordSettings`, `ApplicationUser`, `JwtSettings`

---

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package awisk.Common --version 2.0.11
```

---

## Quick start

- Register database, authentication, and Swagger helpers in your `Program.cs` using the library's service collection extensions.
- Use provided repository bases and migrators for multi-database support (PostgreSQL, MySQL, SQL Server).
- Use `TokenService` and DTOs for token-based authentication flows.

---

## License

See `LICENSE.txt` in the package for license terms.
