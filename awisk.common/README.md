# awisk.common

## Compatible with .NET 9 (net9.0)

`awisk.Common` is a comprehensive shared utility library that provides reusable components, helpers, and extensions commonly used across .NET projects. This package helps ensure consistency, maintainability, and reduces boilerplate code in applications targeting .NET 9.

---

## Features

### 🔐 Authentication and JWT Support
- **TokenService** - JWT token generation and management
- **JwtSettings** - JWT configuration settings
- **AuthConfig** - Authentication configuration
- **AddAuthenticationWrapper** - Easy authentication setup with JWT Bearer and Cookie authentication

### 🌐 API Helpers
- **ApiService** - HTTP client wrapper with support for GET, POST, PUT, DELETE, and file uploads
- **IApiService** - Service interface for API operations

### 📦 DTOs and Responses
- **TokenResponseDto** - JWT token response model
- **ListItemResponseDto** - Generic list item response
- **GenericResponseDto<T>** - Generic response wrapper with status codes
- **PagedRequest** - Request model for pagination (page number, page size, sorting)
- **PagedResponse<T>** - Response model for paginated data with metadata

### 🗄️ Database Support

#### Repositories (Dapper-based)
- **RepositoryBaseSqlServer** - SQL Server repository with full CRUD operations
- **RepositoryBaseMySql** - MySQL repository with full CRUD operations
- **RepositoryBasePostgreSql** - PostgreSQL repository with full CRUD operations
- **IRepositoryBase** - Repository interface with query methods

#### Base Entity Classes
- **BaseGuidEntity** - Base entity with `Guid` primary key (`[ExplicitKey]`)
- **BaseIntEntity** - Base entity with `int` primary key (`[Key]`)
- **BaseLongEntity** - Base entity with `long` primary key (`[Key]`)
- **BaseEntity<T>** - Generic base entity supporting any ID type (int, long, Guid, string, etc.)

#### Entity Framework Core Support
- **ApplicationDbContext** - Base DbContext with Identity support
- **DbContextWrapper** - Generic extension methods for registering DbContext with:
  - SQL Server, MySQL, and PostgreSQL support
  - Generic methods for custom DbContext types
  - Migration assembly support
  - Password settings configuration
  - Both generic and non-generic overloads

#### Database Migrations (FluentMigrator)
- **SqlMigrator** - SQL Server migration runner
- **MySqlMigrator** - MySQL migration runner
- **PostgreSqlMigrator** - PostgreSQL migration runner
- **ExceptionLogMigration** - Exception logging migration
- **MigrationExtensions** - Helper extensions for migrations

### 🛠️ Helpers and Extensions

#### UniversalOperations (Comprehensive Utility Library)
- **UniversalOperations** - Core utilities (Guid, string null checks)
- **UniversalOperations.Strings** - String manipulation, validation, encoding
- **UniversalOperations.Numbers** - Number operations and conversions
- **UniversalOperations.Dates** - Date and time utilities
- **UniversalOperations.Json** - JSON serialization helpers
- **UniversalOperations.Crypto** - Cryptographic operations
- **UniversalOperations.Collections** - Collection utilities
- **UniversalOperations.Parsing** - Type parsing and conversion
- **UniversalOperations.Reflection** - Reflection utilities
- **UniversalOperations.UrlsAndPaths** - URL and path manipulation
- **UniversalOperations.Async** - Async operation helpers
- **UniversalOperations.Performance** - Performance monitoring utilities

#### Other Helpers
- **EnumHelper** - Comprehensive enumeration utilities including:
  - Description attribute support
  - Select list generation for dropdowns
  - Dictionary and tuple conversions
  - Safe parsing with fallback
  - Flags enum support
  - JSON-friendly conversions
- **RandomCodeGenerator** - Random code generation

### 📚 Swagger Integration
- **SwaggerGenWithToken** - Swagger/OpenAPI configuration with JWT Bearer token support
- **SwaggerGen** - Swagger configuration model

### 🔧 Service Registration Helpers
- **DbContextWrapper** - Database context registration extensions
- **AddAuthenticationWrapper** - Authentication setup extensions
- **DbServiceCollection** - Database service collection helpers
- **ServicesCollection** - General service registration helpers
- **ExceptionLogServiceExtensions** - Exception logging service registration
- **ExceptionMiddlewareExtensions** - Global exception handler middleware registration

### ⚙️ Configuration Classes
- **ApplicationSettings** - Main application configuration
- **ApiSettings** - API configuration
- **PasswordSettings** - Password policy settings
- **JwtSettings** - JWT token settings
- **AuthConfig** - Authentication configuration
- **ApplicationUser** - Extended Identity user class
- **ExceptionLog** - Exception log entity for database storage

### 🛡️ Error Handling & Result Pattern
- **Result<T>** - Functional result pattern for explicit error handling
- **Result** - Non-generic result for void operations
- **GlobalExceptionHandlerMiddleware** - ASP.NET Core middleware for global exception handling
- **IExceptionLogService** - Service interface for exception logging
- **ExceptionLogService** - Database-backed exception logging implementation

---

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package awisk.Common --version 2.0.13
```

Or via Package Manager Console:

```powershell
Install-Package awisk.Common -Version 2.0.13
```

---

## Quick Start

### 1. Database Setup

#### Using Entity Framework Core

```csharp
using awisk.common.ServiceCollection;
using awisk.common.Classes;

// SQL Server with default settings
services.AddSqlDbContextDefault<ApplicationDbContext>(connectionString);

// SQL Server with migration assembly
services.AddSqlDbContextDefault<ApplicationDbContext>(connectionString, "YourApp.Migrations");

// SQL Server with custom DbContext and Identity
services.AddSqlDbContextDefault<MyDbContext, MyUser>(connectionString, "YourApp.Migrations");

// MySQL
services.AddMySqlDbContextDefault<ApplicationDbContext>(connectionString, "YourApp.Migrations");

// PostgreSQL
services.AddPostgreSqlDbContextDefault<ApplicationDbContext>(connectionString, "YourApp.Migrations");

// With password settings
var settings = new ApplicationSettings
{
    ConnectionString = connectionString,
    PasswordSettings = new PasswordSettings
    {
        RequireDigit = true,
        RequireLowercase = true,
        RequiredLength = 8
    }
};
services.AddSqlDbContextWithPasswordSettings(settings, "YourApp.Migrations");
```

#### Using Dapper Repositories

```csharp
using awisk.common.Data.Db;

// SQL Server
var repository = new RepositoryBaseSqlServer(connectionString);
var products = repository.GetAll<Product>();

// MySQL
var repository = new RepositoryBaseMySql(connectionString);
var user = repository.GetById<User, int>(userId);

// PostgreSQL
var repository = new RepositoryBasePostgreSql(connectionString);
var result = repository.Query<User>("SELECT * FROM Users WHERE IsActive = @IsActive", 
    new { IsActive = true }, CommandType.Text);
```

### 2. Base Entity Classes

```csharp
using awisk.common.Data.Db;
using Dapper.Contrib.Extensions;

// Using Guid-based entity
public class User : BaseGuidEntity
{
    // Id is already defined as Guid with [ExplicitKey]
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

// Using int-based entity
public class Product : BaseIntEntity
{
    // Id is already defined as int with [Key]
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// Using long-based entity
public class Order : BaseLongEntity
{
    // Id is already defined as long with [Key]
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
}

// Using generic base entity with custom ID type
public class Category : BaseEntity<string>
{
    [ExplicitKey]  // Required for non-auto-increment IDs
    public override string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
```

### 3. Authentication Setup

```csharp
using awisk.common.ServiceCollection;
using awisk.common.Classes;

var jwtSettings = new JwtSettings
{
    Issuer = "YourApp",
    Audience = "YourAppUsers",
    SecretKey = "your-secret-key-here",
    Expiry = 7  // days
};

// For API-only applications
services.AddDefaultAuthenticationForApi(jwtSettings);

// For web applications with cookies
services.AddDefaultAuthentication(jwtSettings);

// With custom configuration
var authConfig = new AuthConfig
{
    AuthScheme = "MyAuthScheme",
    AuthCookie = "MyAuthCookie",
    LoginUrl = "/Account/Login",
    LogoutUrl = "/Account/Logout",
    TokenExpire = 30
};
services.AddDefaultAuthentication(authConfig, jwtSettings);
```

### 4. Token Service

```csharp
using awisk.common.Services;
using awisk.common.Classes;

var tokenService = new TokenService(applicationSettings);

// Generate token from user and roles
var response = tokenService.GenerateToken(user, "Admin,User", "Login successful");

// Generate token string
var tokenString = tokenService.GenerateJwtTokenStr(user, "Admin,User");
```

### 5. API Service

```csharp
using awisk.common.Services;

var apiService = new ApiService(httpClient);

// GET request
var data = await apiService.GetAsync<MyModel>(new Uri("https://api.example.com/data"), bearerToken);

// POST request
var result = await apiService.PostAsync<RequestModel, ResponseModel>(
    new Uri("https://api.example.com/endpoint"),
    requestData,
    bearerToken);

// POST with file upload
var fileResult = await apiService.PostWithFileAsync<RequestModel, ResponseModel>(
    new Uri("https://api.example.com/upload"),
    requestData,
    file,
    "file",
    bearerToken);
```

### 6. Pagination Support

```csharp
using awisk.common.DTOs.Requests;
using awisk.common.DTOs.Responses;

// Create a paged request
var request = new PagedRequest
{
    PageNumber = 1,
    PageSize = 20,
    SortBy = "Name",
    SortDescending = false
};

// Use with repository (example)
var totalCount = repository.Count<Product>();
var products = repository.Query<Product>(
    $"SELECT * FROM Products ORDER BY {request.SortBy} {(request.SortDescending ? "DESC" : "ASC")} OFFSET {request.Skip} ROWS FETCH NEXT {request.Take} ROWS ONLY",
    null,
    System.Data.CommandType.Text);

// Create paged response
var response = new PagedResponse<Product>(products, request, totalCount);

// Response contains:
// - response.Data: List of products
// - response.PageNumber: Current page (1)
// - response.PageSize: Items per page (20)
// - response.TotalCount: Total items
// - response.TotalPages: Calculated total pages
// - response.HasPreviousPage: false (first page)
// - response.HasNextPage: true/false
```

### 7. Result Pattern

```csharp
using awisk.common.Common;

// For operations that return a value
public Result<User> GetUser(int id)
{
    var user = repository.GetById<User, int>(id);
    if (user == null)
        return Result<User>.Failure("User not found");
    
    return Result<User>.Success(user);
}

// Usage
var result = GetUser(123);
if (result.IsSuccess)
{
    var user = result.Value!; // Safe to use
    Console.WriteLine(user.Name);
}
else
{
    Console.WriteLine($"Error: {result.ErrorMessage}");
}

// For void operations
public Result DeleteUser(int id)
{
    var user = repository.GetById<User, int>(id);
    if (user == null)
        return Result.Failure("User not found");
    
    repository.Delete(user);
    return Result.Success();
}

// Implicit conversions
Result<int> successResult = 42; // Automatically creates Success(42)
Result<string> failureResult = "Error message"; // Automatically creates Failure("Error message")
```

### 8. Global Exception Handling & Exception Logging

```csharp
using awisk.common.ServiceCollection;
using awisk.common.Middleware;

// Register exception logging service (requires IRepositoryBase to be registered)
services.AddExceptionLogService();

// Add global exception handler middleware
app.UseGlobalExceptionHandler();

// The middleware will:
// - Catch all unhandled exceptions
// - Return consistent error responses as GenericResponseDto<object>
// - Log exceptions to console/application logs
// - Log exceptions to database (if ExceptionLogService is registered)
// - Map exception types to appropriate HTTP status codes:
//   - ArgumentException/ArgumentNullException → 400 Bad Request
//   - UnauthorizedAccessException → 401 Unauthorized
//   - KeyNotFoundException/FileNotFoundException → 404 Not Found
//   - NotImplementedException → 501 Not Implemented
//   - TimeoutException → 408 Request Timeout
//   - Other exceptions → 500 Internal Server Error

// Manual exception logging
var exceptionLogService = serviceProvider.GetRequiredService<IExceptionLogService>();
await exceptionLogService.LogExceptionAsync(exception, "https://api.example.com/users");
```

### 9. Swagger Configuration

```csharp
using awisk.common.ServiceCollection;
using awisk.common.Classes;

var swaggerConfig = new SwaggerGen
{
    Title = "My API",
    Version = "v1",
    Description = "API Documentation",
    ContactName = "Support Team",
    ContactEmail = "support@example.com",
    ContactUrl = "https://example.com/support"
};

services.InitSwaggerGenWithToken(swaggerConfig);
```

### 7. Universal Operations Helpers

```csharp
using awisk.common.Helpers;

// String operations
var safeString = someString.IfNullEmptyString();
var trimmed = someString.TrimSafe();
var isValid = email.IsValidEmail();
var slug = title.ToSlug();

// Guid operations
var newGuid = UniversalOpertaions.NewGuid();
var guidString = UniversalOpertaions.NewGuidStr();
var isEmpty = myGuid.IsEmpty();

// Date operations
var utcNow = UniversalOpertaions.GetUtcNow();
var formatted = date.ToFormattedString("yyyy-MM-dd");

// Number operations
var parsed = UniversalOpertaions.ToInt("123", 0);
var isNumeric = "123".IsNumeric();
```

### 8. Enum Helper

```csharp
using awisk.common.Helpers;
using System.ComponentModel;

// Define an enum with descriptions
public enum UserStatus
{
    [Description("Active User")]
    Active = 1,
    [Description("Inactive User")]
    Inactive = 2,
    [Description("Pending Verification")]
    Pending = 3
}

// Get description from enum value
var status = UserStatus.Active;
var description = status.ToDescription(); // Returns "Active User"

// Get enum from description
var parsedStatus = EnumHelper.FromDescription<UserStatus>("Active User");

// Get select list for dropdowns
var selectList = EnumHelper.GetSelectListFromEnum<UserStatus>();
// Returns IEnumerable<ListItemResponseDto<UserStatus>> with Id and Value (description)

// Get text select list (uses enum name instead of description)
var textList = EnumHelper.GetTextSelectListFromEnum<UserStatus>();

// Convert to dictionary (int -> description)
var dict = EnumHelper.ToDictionary<UserStatus>();
// { 1: "Active User", 2: "Inactive User", 3: "Pending Verification" }

// Convert to tuple list
var tuples = EnumHelper.ToTupleList<UserStatus>();
// Returns: [(1, "Active User"), (2, "Inactive User"), (3, "Pending Verification")]

// Get all names
var names = EnumHelper.GetNames<UserStatus>(); // ["Active", "Inactive", "Pending"]

// Get all values
var values = EnumHelper.GetValues<UserStatus>(); // [UserStatus.Active, UserStatus.Inactive, ...]

// Safe parsing with fallback
var parsed = EnumHelper.ParseSafe<UserStatus>("Invalid", UserStatus.Active);
// Returns UserStatus.Active if parsing fails

// Convert enum to int
var intValue = UserStatus.Active.ToInt(); // Returns 1

// Check if integer is valid enum value
var isValid = EnumHelper.IsDefined<UserStatus>(1); // Returns true

// JSON-friendly list
var jsonList = EnumHelper.ToJsonList<UserStatus>();
// Returns: [{ id: 1, value: "Active User" }, { id: 2, value: "Inactive User" }, ...]

// Flags enum support
[Flags]
public enum Permissions
{
    [Description("Read Access")]
    Read = 1,
    [Description("Write Access")]
    Write = 2,
    [Description("Delete Access")]
    Delete = 4
}

var permissions = Permissions.Read | Permissions.Write;
var hasRead = permissions.HasFlagFast(Permissions.Read); // Returns true
var combined = permissions.ToCombinedDescription(); // Returns "Read Access, Write Access"
```

---

## Database Migrations

### Running Migrations

The library includes FluentMigrator-based migration runners for each database:

```csharp
// SQL Server
SqlMigrator.Migrator(args, "YourApp.Migrations", "YourApp.Migrator");

// MySQL
MySqlMigrator.Migrator(args, "YourApp.Migrations", "YourApp.Migrator");

// PostgreSQL
PostgreSqlMigrator.Migrator(args, "YourApp.Migrations", "YourApp.Migrator");
```

Command-line usage:
```bash
YourApp.Migrator.exe server database -u username -p password -m up -q
```

---

## Repository Methods

All repository classes support the following operations:

- `GetAll<T>()` - Get all entities
- `GetById<T, ID>(ID id)` - Get entity by ID
- `Insert<T>(T item)` - Insert single entity
- `Insert<T>(IEnumerable<T> items)` - Insert multiple entities
- `Update<T>(T item)` - Update entity
- `Delete<T, ID>(ID id)` - Delete by ID
- `Delete<T>(T item)` - Delete entity
- `Execute(sql, parameters, commandType)` - Execute SQL command
- `Query<T>(sql, parameters, commandType)` - Query with typed results
- `QueryFirst<T>(sql, parameters, commandType)` - Get first result
- `QueryFirstOrDefault<T>(sql, parameters, commandType)` - Get first or default
- `QuerySingle<T>(sql, parameters, commandType)` - Get single result
- `QuerySingleOrDefault<T>(sql, parameters, commandType)` - Get single or default
- `DeleteByIdAsync(sql, parameters, commandType)` - Async delete

### Paging Support

```csharp
// SQL Server
var paging = RepositoryBaseSqlServer.GetPagingStatement(page: 2, pageSize: 20);
// Returns: " OFFSET 20 ROWS FETCH NEXT 20 ROWS ONLY"

// MySQL
var paging = RepositoryBaseMySql.GetPagingStatement(page: 2, pageSize: 20);
// Returns: " LIMIT 20 OFFSET 20"

// PostgreSQL
var paging = RepositoryBasePostgreSql.GetPagingStatement(page: 2, pageSize: 20);
// Returns: " LIMIT 20 OFFSET 20"
```

---

## Requirements

- .NET 9.0 or later
- SQL Server, MySQL, or PostgreSQL database
- (Optional) Entity Framework Core 9.0 for EF Core features
- (Optional) FluentMigrator 7.1.0 for migrations

---

## Key Improvements in v2.0.13

- ✅ Generic DbContext registration methods for all database providers
- ✅ Migration assembly support in all DbContext methods
- ✅ Fixed paging SQL syntax for MySQL and PostgreSQL
- ✅ Improved null safety in repository methods
- ✅ Enhanced ApiService with proper header management
- ✅ Fixed string splitting in TokenService
- ✅ BaseGuidEntity, BaseIntEntity, BaseLongEntity for consistent entity base classes
- ✅ Generic BaseEntity<T> for flexible ID types
- ✅ Comprehensive UniversalOperations helper library

---

## License

See `LICENSE.txt` in the package for license terms.

---

## Support

For issues, questions, or contributions, please refer to the project repository or contact the maintainers.

---

**Version:** 2.0.13  
**Author:** Syed Ali Hassan  
**Company:** awisk
