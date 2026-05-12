# Changelog

All notable changes to **awisk.Common** are documented here.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [3.0.0] — 2026-05-12

### Breaking Changes

- **Target framework upgraded from .NET 9 to .NET 10.** All Microsoft.* packages updated to `10.0.7`.
- All async repository methods now require `CancellationToken ct = default` — existing callers without a token continue to work unchanged (default parameter), but subclasses overriding these methods must update their signatures.
- `BaseEntity<T>.UpdatedOn` and `DeletedOn` changed from sentinel `DateTime(1900,01,01)` to `DateTime?` (nullable). Run the included migration scripts before deploying.
- `IApiService.PutAsync` no longer has a `new()` generic constraint on `TResponse`. The silent `return new()` on failure was replaced with `EnsureSuccessStatusCode()` — callers that relied on a default empty object on failure must handle the exception.

### Added

- **`ICurrentUserService` / `CurrentUserService`** — extracts `UserId`, `Email`, `FullName`, `Roles`, `IsAuthenticated` from `ClaimsPrincipal` via `IHttpContextAccessor`.
- **`ICacheService` / `MemoryCacheService`** — `IMemoryCache` wrapper with `GetAsync`, `SetAsync`, `RemoveAsync`, `GetOrSetAsync`.
- **`ICacheService` / `DistributedCacheService`** — `IDistributedCache` (Redis) wrapper with JSON serialization.
- **`IEmailService` / `SmtpEmailService`** — MailKit SMTP client supporting `To`, `Cc`, `Bcc`, `Subject`, `Body`, `IsHtml`, and file attachments (`EmailAttachment`).
- **`IFileStorageService` / `LocalFileStorageService`** — local disk file storage with save, get, delete, exists, and public URL generation. Configurable via `FileStorageSettings`.
- **`ICorrelationIdService` / `CorrelationIdService`** — reads `X-Correlation-ID` from `HttpContext.Items`.
- **`CorrelationIdMiddleware`** — reads or generates `X-Correlation-ID` on every inbound request and echoes it in the response header.
- **`RequestResponseLoggingMiddleware`** — structured logging of HTTP method, path, query string, status code, and elapsed milliseconds with correlation ID.
- **`AuditInterceptor`** — EF Core `SaveChangesInterceptor` that auto-stamps `CreatedBy`, `CreatedOn`, `UpdatedBy`, `UpdatedOn` from `ICurrentUserService` on every save.
- **`ModelBuilderExtensions.ApplySoftDeleteFilter()`** — registers a global EF Core `HasQueryFilter(e => !e.IsDeleted)` for all entities inheriting from `BaseEntity<T>`.
- **DI extension methods** — `AddCurrentUserService()`, `AddMemoryCacheService()`, `AddDistributedCacheService()`, `AddSmtpEmailService()`, `AddLocalFileStorage()`, `AddCorrelationId()` / `UseCorrelationId()`, `UseRequestResponseLogging()`.
- **`HealthCheckExtensions`** — `AddSqlServerHealthCheck()`, `AddMySqlHealthCheck()`, `AddPostgreSqlHealthCheck()` convenience wrappers.
- **`EmailMessage`** / **`EmailSettings`** / **`FileStorageSettings`** configuration classes.
- **`CancellationToken ct = default`** added to every async method in `IRepositoryBase` and all three repository implementations. Tokens are propagated through Dapper `CommandDefinition` and `OpenAsync(ct)`.
- **`Result<T>.Map<TNew>()`** — transforms the success value while preserving failure state.
- **`Result<T>.Bind<TNew>()`** — chains operations that return `Result<T>`.
- **`Result<T>.Match<TOut>()`** — exhaustive pattern match over success / failure.
- **`EnumHelper`** description caching via `ConcurrentDictionary` for thread-safe, allocation-free repeated lookups.
- Database migration scripts for the `UpdatedOn` / `DeletedOn` nullable column change (`improvements/migrations/sqlserver_nullable_dates.sql`, `mysql_nullable_dates.sql`, `postgresql_nullable_dates.sql`).

### Changed

- Upgraded `Swashbuckle.AspNetCore` to `10.1.7` and `Microsoft.OpenApi` to `3.5.3`; fixed all resulting breaking API changes (`OpenApiSecuritySchemeReference`, `AddSecurityRequirement` delegate signature, `List<string>` scope collection).
- All Dapper `CommandDefinition` constructions now use named arguments for clarity.
- `GlobalExceptionHandlerMiddleware` — `JsonSerializerOptions` promoted to `private static readonly` to avoid per-request allocation.
- `RepositoryBase` — `CreateBatches<T>` refactored from O(n²) manual chunking to `items.Chunk(batchSize)`.
- `RepositoryBase` — `Delete<T, ID>` / `DeleteAsync<T, ID>` reduced from two round-trips (fetch then delete) to a single `DELETE FROM ... WHERE Id = @Id`.
- `RepositoryBase` — `Exists<T, ID>` / `ExistsAsync<T, ID>` changed from a full entity fetch to `SELECT COUNT(1) ... WHERE Id = @Id`.
- `TokenService.CreateClaims` — removed self-referential `new Claim("Token", response.Token)` claim that embedded the raw JWT inside itself.
- `PagedResponse<T>` — divide-by-zero guard on `TotalPages` now also checks `PageSize > 0`.

### Removed

- Silent `return new TResponse()` fallback in `ApiService.PutAsync` on non-success HTTP status — replaced with `EnsureSuccessStatusCode()`.

### Dependencies Updated

| Package | From | To |
|---|---|---|
| Microsoft.EntityFrameworkCore.* | 9.x | 10.0.7 |
| Microsoft.AspNetCore.* | 9.x | 10.0.7 |
| Microsoft.Data.SqlClient | 5.x | 7.0.1 |
| Npgsql | 8.x | 10.0.2 |
| Npgsql.EntityFrameworkCore.PostgreSQL | 8.x | 10.0.1 |
| FluentMigrator | 5.x | 8.0.1 |
| MySql.Data | 8.x | 9.7.0 |
| Swashbuckle.AspNetCore | 6.x | 10.1.7 |
| Microsoft.OpenApi | 1.x | 3.5.3 |
| MailKit | — | 4.16.0 *(new)* |
| Microsoft.Extensions.Caching.Memory | — | 10.0.7 *(new)* |
| Microsoft.Extensions.Caching.StackExchangeRedis | — | 10.0.7 *(new)* |
| StackExchange.Redis | — | 2.12.14 *(new)* |
| AspNetCore.HealthChecks.SqlServer | — | 9.0.0 *(new)* |
| AspNetCore.HealthChecks.MySql | — | 9.0.0 *(new)* |
| AspNetCore.HealthChecks.NpgSql | — | 9.0.0 *(new)* |

> **Note:** `Pomelo.EntityFrameworkCore.MySql` remains at `9.0.0` as no .NET 10 compatible release is available yet. NU1608 warning is expected and can be suppressed.

---

## [2.0.14] — 2025-05-01

### Added

- `Result<T>` functional result type with `IsSuccess`, `Value`, `ErrorMessage`, `Exception` properties and implicit conversion operators.
- `Result` (non-generic) for void operations.
- `PagedRequest` / `PagedResponse<T>` pagination DTOs.
- `GlobalExceptionHandlerMiddleware` with structured JSON error responses and exception-to-status-code mapping.
- `ExceptionLogServiceExtensions` / `ExceptionMiddlewareExtensions` DI helpers.
- Database migration scripts for nullable date columns.

### Changed

- `BaseEntity<T>` — `UpdatedOn` and `DeletedOn` changed from `DateTime(1900,01,01)` to `DateTime?`.
- `RepositoryBase` — `CreateBatches<T>` now uses `Enumerable.Chunk()` (O(n) instead of O(n²)).
- `RepositoryBase` — `Delete` and `Exists` operations reduced to single SQL round-trips.
- `RepositoryBase` — Added `ExecuteInTransactionAsync` overloads for both `Task<T>` and `Task` work.
- `RepositoryBase` — Added `GetTableName<T>()` helper reading the `[Table]` attribute.
- `EnumHelper` — added `ConcurrentDictionary` description cache.
- `ApiService.PutAsync` — removed silent `return new()` fallback.
- `TokenService` — removed redundant self-referential token claim.
- `PagedResponse<T>` — fixed divide-by-zero when `TotalCount` or `PageSize` is zero.

---

## [2.0.13] — 2025-04-01

### Changed

- Simplified Swagger configuration.
- General package updates to the latest .NET 9 compatible versions.

---

## [2.0.12] — 2025-03-15

### Added

- Added result pattern foundation.
- Added pagination utilities.

---

## [2.0.11] — 2025-03-01

### Changed

- Enhanced API service with improved HTTP client handling.
- Enhanced token service with configurable claim generation.
- General code quality improvements across services.

---

## [2.0.10] — 2025-02-15

### Changed

- Version bump with stability improvements.

---

## [2.0.8] — 2025-02-01

### Added

- File upload handling support.
- `PostWithFileAsync` method added to `IApiService`.

---

## [2.0.5 – 2.0.7] — 2025-01-01

### Added

- Incremental improvements to base entity generic support.
- New utility and helper methods in `UniversalOperations`.

---

## [2.0.1] — 2024-12-01

### Added

- Initial .NET 9 release.
- Dapper repository pattern for SQL Server, MySQL, and PostgreSQL.
- `BaseEntity<T>`, `BaseIntEntity`, `BaseGuidEntity`, `BaseLongEntity`.
- JWT authentication wrapper.
- Swagger with JWT bearer configuration.
- `IApiService` / `ApiService` HTTP client abstraction.
- `ITokenService` / `TokenService` JWT generation.
- `IExceptionLogService` / `ExceptionLogService` database exception logging.
- `EnumHelper` with dropdown, description, parsing, and flag utilities.
- `UniversalOperations` helpers (strings, dates, crypto, JSON, reflection, collections, URLs).
- FluentMigrator integration with SQL Server, MySQL, and PostgreSQL runners.
- `ApplicationDbContext` with ASP.NET Core Identity support.

---

## [1.0.2] — 2024-11-01

### Added

- Initial public release targeting .NET 9 (pre-release).
- Core Dapper repository and base entity patterns.
