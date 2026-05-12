# awisk.Common

A comprehensive .NET shared utility library providing common infrastructure patterns, services, and helpers for building ASP.NET Core applications. Designed to eliminate boilerplate across projects.

[![NuGet](https://img.shields.io/nuget/v/awisk.Common.svg)](https://www.nuget.org/packages/awisk.Common)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.txt)

---

## Table of Contents

- [Installation](#installation)
- [Requirements](#requirements)
- [Features](#features)
- [Quick Start](#quick-start)
- [Core Features](#core-features)
  - [Result Pattern](#result-pattern)
  - [Pagination](#pagination)
  - [Repository Pattern (Dapper)](#repository-pattern-dapper)
  - [Current User Service](#current-user-service)
  - [Caching](#caching)
  - [Email Service](#email-service)
  - [File Storage](#file-storage)
  - [Correlation ID](#correlation-id)
  - [Request/Response Logging](#requestresponse-logging)
  - [Global Exception Handling](#global-exception-handling)
  - [JWT Authentication](#jwt-authentication)
  - [EF Core Audit & Soft Delete](#ef-core-audit--soft-delete)
  - [Health Checks](#health-checks)
  - [Enum Helpers](#enum-helpers)
  - [Database Migrations](#database-migrations)
- [Configuration Reference](#configuration-reference)
- [Base Entities](#base-entities)

---

## Installation

```bash
dotnet add package awisk.Common
```

Or via the NuGet Package Manager:

```
Install-Package awisk.Common
```

---

## Requirements

- .NET 10.0+
- ASP.NET Core 10.0+

---

## Features

| Category | What's included |
|---|---|
| **Error Handling** | Result pattern (`Result<T>`), global exception middleware |
| **Repositories** | Dapper-based generic repositories for SQL Server, MySQL, PostgreSQL |
| **Caching** | `ICacheService` with in-memory and Redis implementations |
| **Email** | `IEmailService` with SMTP via MailKit, attachment support |
| **File Storage** | `IFileStorageService` with local disk implementation |
| **Auth** | JWT bearer setup, `ICurrentUserService`, `ITokenService` |
| **Middleware** | Correlation ID, request/response logging, global exception handler |
| **EF Core** | Audit interceptor, soft-delete global query filter |
| **Pagination** | `PagedRequest` / `PagedResponse<T>` DTOs |
| **Migrations** | FluentMigrator runners for SQL Server, MySQL, PostgreSQL |
| **Swagger** | Pre-configured JWT bearer Swagger setup |
| **Health Checks** | Built-in checks for SQL Server, MySQL, PostgreSQL |
| **Helpers** | EnumHelper, UniversalOperations (strings, dates, crypto, JSON, etc.) |

---

## Quick Start

Register all core services in `Program.cs`:

```csharp
using awisk.common.ServiceCollection;

var builder = WebApplication.CreateBuilder(args);

// Core services
builder.Services.RegisterServices();

// Current user (requires IHttpContextAccessor)
builder.Services.AddCurrentUserService();

// In-memory cache
builder.Services.AddMemoryCacheService();

// Redis cache (alternative)
// builder.Services.AddDistributedCacheService("your-redis-connection-string");

// Email
builder.Services.AddSmtpEmailService(builder.Configuration);

// Local file storage
builder.Services.AddLocalFileStorage(builder.Configuration);

// Correlation ID
builder.Services.AddCorrelationId();

// Health checks
builder.Services.AddHealthChecks()
    .AddSqlServerHealthCheck(builder.Configuration.GetConnectionString("Default")!);

// JWT authentication
builder.Services.AddAuthenticationWrapper(builder.Configuration);

// Swagger with JWT bearer
builder.Services.AddSwaggerWithJwtBearer();

var app = builder.Build();

// Middleware pipeline order matters
app.UseCorrelationId();
app.UseRequestResponseLogging();
app.UseGlobalExceptionHandler();

app.MapHealthChecks("/health");
app.Run();
```

---

## Core Features

### Result Pattern

`Result<T>` is a functional error-handling type that avoids exceptions for expected failure paths.

```csharp
// Returning results
public Result<User> GetUser(int id)
{
    var user = _db.Find(id);
    if (user is null)
        return Result<User>.Failure("User not found.");

    return Result<User>.Success(user);
}

// Consuming results
var result = GetUser(42);

if (result.IsSuccess)
    Console.WriteLine(result.Value!.Name);
else
    Console.WriteLine(result.ErrorMessage);

// Pattern matching
var name = result.Match(
    onSuccess: user => user.Name,
    onFailure: error => $"Error: {error}"
);

// Chaining with Map / Bind
var nameResult = GetUser(42)
    .Map(user => user.Name.ToUpperInvariant());

var addressResult = GetUser(42)
    .Bind(user => GetAddress(user.AddressId));

// Implicit conversions
Result<string> ok = "hello";       // Success
Result<string> fail = "Not found"; // Failure

// Void results
public Result DoSomething()
{
    // ...
    return Result.Success();
    // or
    return Result.Failure("Something went wrong.");
}
```

---

### Pagination

```csharp
// Request (from query string)
public IActionResult GetUsers([FromQuery] PagedRequest request)
{
    // request.Skip / request.Take for database queries
    var users = _repo.GetAll()
        .Skip(request.Skip)
        .Take(request.Take)
        .ToList();

    int total = _repo.Count();

    return Ok(new PagedResponse<User>(users, request, total));
}
```

`PagedRequest` defaults: `PageNumber = 1`, `PageSize = 10`, max page size 1000. Supports optional `SortBy` and `SortDescending`.

`PagedResponse<T>` exposes `TotalPages`, `HasPreviousPage`, `HasNextPage` computed properties.

---

### Repository Pattern (Dapper)

Three database-specific implementations share a common `IRepositoryBase` interface.

```csharp
// Inject IRepositoryBase or extend a concrete class
public class UserRepository : RepositoryBaseSqlServer
{
    public UserRepository(string connectionString) : base(connectionString) { }

    public async Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken ct = default)
        => await QueryAsync<User>(
            "SELECT * FROM Users WHERE IsActive = 1",
            parameters: null,
            CommandType.Text,
            ct);
}
```

**Available operations:**

```csharp
// CRUD
await repo.GetAllAsync<User>(ct);
await repo.GetByIdAsync<User, int>(id, ct);
await repo.InsertAsync(user, ct);
await repo.InsertAsync(userList, ct);
await repo.UpdateAsync(user, ct);
await repo.DeleteAsync<User, int>(id, ct);

// Queries (stored procedures or raw SQL)
await repo.QueryAsync<User>(sql, parameters, CommandType.Text, ct);
await repo.QueryFirstOrDefaultAsync<User>(sql, parameters, CommandType.StoredProcedure, ct);
await repo.ExecuteAsync(sql, parameters, CommandType.Text, ct);

// Count & Exists
await repo.CountAsync<User>(ct);
await repo.ExistsAsync<User, int>(id, ct);

// Transactions
await repo.ExecuteInTransactionAsync(async (conn, tx) =>
{
    await conn.ExecuteAsync("INSERT ...", transaction: tx);
    await conn.ExecuteAsync("UPDATE ...", transaction: tx);
}, ct);

// Batch insert helper
var batches = RepositoryBaseSqlServer.CreateBatches(largeList, batchSize: 2000);
foreach (var batch in batches)
    await repo.InsertAsync(batch, ct);
```

**Paging helpers:**

```csharp
// SQL Server
var paging = RepositoryBaseSqlServer.GetPagingStatement(page: 2, pageSize: 20);
// Appends: OFFSET 20 ROWS FETCH NEXT 20 ROWS ONLY

// MySQL / PostgreSQL
var paging = RepositoryBaseMySql.GetPagingStatement(page: 2, pageSize: 20);
// Appends: LIMIT 20 OFFSET 20
```

---

### Current User Service

Extracts the authenticated user's claims from the current HTTP context.

```csharp
// Registration
builder.Services.AddCurrentUserService();

// Usage
public class OrderService(ICurrentUserService currentUser)
{
    public void PlaceOrder(Order order)
    {
        order.CreatedBy = currentUser.UserId;
        order.CustomerEmail = currentUser.Email;

        if (!currentUser.IsInRole("Customer"))
            throw new UnauthorizedAccessException();
    }
}
```

**Available properties:** `UserId`, `Email`, `FullName`, `Roles`, `IsAuthenticated`, `IsInRole(role)`.

---

### Caching

**In-memory cache:**

```csharp
builder.Services.AddMemoryCacheService();
```

**Redis distributed cache:**

```csharp
builder.Services.AddDistributedCacheService("localhost:6379");
```

**Usage (same interface for both):**

```csharp
public class ProductService(ICacheService cache)
{
    private static readonly TimeSpan _ttl = TimeSpan.FromMinutes(10);

    public async Task<Product?> GetProductAsync(int id, CancellationToken ct)
    {
        return await cache.GetOrSetAsync(
            key: $"product:{id}",
            factory: () => _db.GetProductAsync(id, ct),
            expiration: _ttl,
            ct: ct);
    }

    public async Task InvalidateAsync(int id, CancellationToken ct)
        => await cache.RemoveAsync($"product:{id}", ct);
}
```

---

### Email Service

**Configuration (`appsettings.json`):**

```json
{
  "EmailSettings": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "user@example.com",
    "Password": "your-password",
    "FromAddress": "noreply@example.com",
    "FromName": "My App",
    "UseSsl": true
  }
}
```

**Registration:**

```csharp
builder.Services.AddSmtpEmailService(builder.Configuration);
// or with inline config:
builder.Services.AddSmtpEmailService(opt =>
{
    opt.Host = "smtp.example.com";
    opt.Port = 587;
    // ...
});
```

**Usage:**

```csharp
// Simple
await emailService.SendAsync("user@example.com", "Welcome!", "<h1>Hello</h1>", isHtml: true, ct);

// Full control with attachments
var message = new EmailMessage
{
    To = ["alice@example.com", "bob@example.com"],
    Cc = ["manager@example.com"],
    Subject = "Monthly Report",
    Body = "<p>Please find the report attached.</p>",
    IsHtml = true,
    Attachments =
    [
        new EmailAttachment
        {
            FileName = "report.pdf",
            Content = pdfStream,
            ContentType = "application/pdf"
        }
    ]
};

await emailService.SendAsync(message, ct);
```

---

### File Storage

**Configuration (`appsettings.json`):**

```json
{
  "FileStorageSettings": {
    "BasePath": "C:\\uploads",
    "BaseUrl": "https://yourdomain.com/files",
    "MaxFileSizeBytes": 10485760,
    "AllowedExtensions": [".jpg", ".png", ".pdf"]
  }
}
```

**Registration:**

```csharp
builder.Services.AddLocalFileStorage(builder.Configuration);
```

**Usage:**

```csharp
public class DocumentController(IFileStorageService storage) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        var path = await storage.SaveAsync(stream, file.FileName, folder: "documents", ct);
        var url = storage.GetPublicUrl(path);
        return Ok(new { url });
    }

    [HttpGet("{path}")]
    public async Task<IActionResult> Download(string path, CancellationToken ct)
    {
        var stream = await storage.GetAsync(path, ct);
        if (stream is null) return NotFound();
        return File(stream, "application/octet-stream");
    }
}
```

---

### Correlation ID

Attaches a unique `X-Correlation-ID` header to every request and propagates it through the response.

```csharp
// Registration
builder.Services.AddCorrelationId();

// Middleware (add early in the pipeline)
app.UseCorrelationId();

// Access in services
public class OrderService(ICorrelationIdService correlationId, ILogger<OrderService> logger)
{
    public void Process()
    {
        logger.LogInformation("Processing order [CorrelationId: {Id}]", correlationId.CorrelationId);
    }
}
```

---

### Request/Response Logging

Logs every HTTP request with method, path, status code, and elapsed time.

```csharp
app.UseRequestResponseLogging();
```

Output example:
```
HTTP GET /api/users?page=1 started [CorrelationId: abc-123]
HTTP GET /api/users responded 200 in 45ms [CorrelationId: abc-123]
```

---

### Global Exception Handling

Catches all unhandled exceptions and returns consistent JSON error responses.

```csharp
app.UseGlobalExceptionHandler();
```

| Exception | HTTP Status |
|---|---|
| `ArgumentException`, `ArgumentNullException` | 400 Bad Request |
| `UnauthorizedAccessException` | 401 Unauthorized |
| `KeyNotFoundException`, `FileNotFoundException` | 404 Not Found |
| `TimeoutException` | 408 Request Timeout |
| `NotImplementedException` | 501 Not Implemented |
| All others | 500 Internal Server Error |

Response format:

```json
{
  "statusCode": 404,
  "message": "User not found."
}
```

---

### JWT Authentication

```csharp
// Registration
builder.Services.AddAuthenticationWrapper(builder.Configuration);
```

**Configuration (`appsettings.json`):**

```json
{
  "JwtSettings": {
    "Secret": "your-256-bit-secret-key",
    "Issuer": "your-app",
    "Audience": "your-api",
    "ExpiryMinutes": 60
  }
}
```

**Token generation:**

```csharp
public class AuthController(ITokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // Validate credentials...
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, "Admin")
        };

        var token = tokenService.GenerateToken(claims);
        return Ok(new { token });
    }
}
```

---

### EF Core Audit & Soft Delete

**Audit Interceptor** — automatically stamps `CreatedBy`, `CreatedOn`, `UpdatedBy`, `UpdatedOn` on every save.

```csharp
// Register in DbContext options
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseSqlServer(connectionString)
           .AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
});

// Register the interceptor itself
builder.Services.AddCurrentUserService();
builder.Services.AddScoped<AuditInterceptor>();
```

**Soft Delete Filter** — automatically excludes soft-deleted rows from all queries for entities inheriting from `BaseEntity<T>`.

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplySoftDeleteFilter();
    base.OnModelCreating(modelBuilder);
}
```

---

### Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddSqlServerHealthCheck(connectionString)           // name: "sqlserver"
    .AddMySqlHealthCheck(connectionString)               // name: "mysql"
    .AddPostgreSqlHealthCheck(connectionString);         // name: "postgresql"

app.MapHealthChecks("/health");
```

---

### Enum Helpers

```csharp
// Enum with descriptions
public enum Status
{
    [Description("Active User")] Active,
    [Description("Inactive User")] Inactive,
    [Description("Pending Review")] Pending
}

// Get select list for dropdowns
var list = EnumHelper.GetSelectListFromEnum<Status>();
// [{ Value: 0, Text: "Active User" }, ...]

// Extension methods
Status.Active.ToDescription();       // "Active User"
Status.Active.ToInt();               // 0
EnumHelper.FromDescription<Status>("Active User"); // Status.Active

// Safe parsing with fallback
var status = EnumHelper.ParseSafe<Status>("Unknown", Status.Active); // Status.Active

// Flags enums
[Flags] public enum Permissions { Read = 1, Write = 2, Delete = 4 }
var perms = Permissions.Read | Permissions.Write;
perms.ToCombinedDescription(); // "Read, Write"
perms.HasFlagFast(Permissions.Read); // true
```

---

### Database Migrations

Built on FluentMigrator. Run migrations at startup or via CLI.

```csharp
// Registration
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(runner => runner
        .AddSqlServer()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());

// Run on startup
app.RunMigrations(); // extension method from MigrationExtensions
```

---

## Configuration Reference

### `appsettings.json` full example

```json
{
  "JwtSettings": {
    "Secret": "your-256-bit-secret",
    "Issuer": "my-app",
    "Audience": "my-api",
    "ExpiryMinutes": 60
  },
  "EmailSettings": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "smtp-user",
    "Password": "smtp-password",
    "FromAddress": "noreply@example.com",
    "FromName": "My Application",
    "UseSsl": true
  },
  "FileStorageSettings": {
    "BasePath": "/var/app/uploads",
    "BaseUrl": "https://yourdomain.com/files",
    "MaxFileSizeBytes": 10485760,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx"]
  }
}
```

---

## Base Entities

All base entities include full audit trail and soft-delete support.

| Property | Type | Description |
|---|---|---|
| `Id` | `T` | Primary key (type varies by base class) |
| `CreatedBy` | `string` | Who created the record |
| `CreatedOn` | `DateTime` | When the record was created |
| `UpdatedBy` | `string` | Who last updated the record |
| `UpdatedOn` | `DateTime?` | When the record was last updated |
| `DeletedBy` | `string` | Who soft-deleted the record |
| `DeletedOn` | `DateTime?` | When the record was soft-deleted |
| `IsActive` | `bool` | Active flag (default: `true`) |
| `IsDeleted` | `bool` | Soft-delete flag (default: `false`) |

**Available base classes:**

```csharp
public class Product : BaseIntEntity  { }  // int Id (auto-increment)
public class Order   : BaseLongEntity { }  // long Id (auto-increment)
public class Session : BaseGuidEntity { }  // Guid Id (auto-generated)

// Or use the generic base directly with a custom key type
public class Custom : BaseEntity<string> { }
```

---

## License

This project is licensed under the MIT License. See [LICENSE.txt](LICENSE.txt) for details.

---

*Maintained by [awisk](https://github.com/awisk) · Built with .NET 10*
