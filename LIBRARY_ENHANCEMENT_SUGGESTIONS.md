# Library Enhancement Suggestions

This document outlines valuable additions that would make `awisk.common` more comprehensive and useful for consumers.

---

## 🔥 High-Priority Additions

### 1. **Pagination Support**
**Why:** Essential for API responses and data grids.

**Suggested Implementation:**
```csharp
// DTOs/Requests/PagedRequest.cs
public class PagedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

// DTOs/Responses/PagedResponse.cs
public class PagedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
```

**Benefits:**
- Standardized pagination across all projects
- Consistent API responses
- Easy to use with existing repositories

---

### 2. **Result<T> Pattern**
**Why:** Better error handling than exceptions for expected failures.

**Suggested Implementation:**
```csharp
// Common/Result.cs
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public Exception? Exception { get; }
    
    private Result(bool isSuccess, T? value, string? errorMessage, Exception? exception)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        Exception = exception;
    }
    
    public static Result<T> Success(T value) => new(true, value, null, null);
    public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage, null);
    public static Result<T> Failure(Exception exception) => new(false, default, exception.Message, exception);
}

public class Result : Result<object>
{
    public static Result Success() => new(true, null, null, null);
    public static Result Failure(string errorMessage) => new(false, null, errorMessage, null);
}
```

**Benefits:**
- Explicit error handling
- No exception overhead for expected failures
- Functional programming style

---

### 3. **Exception Logging Service**
**Why:** You already have `ExceptionLogMigration` but no service to use it.

**Suggested Implementation:**
```csharp
// Interfaces/IExceptionLogService.cs
public interface IExceptionLogService
{
    Task LogExceptionAsync(Exception exception, string? url = null, CancellationToken ct = default);
    Task LogExceptionAsync(string message, string? stackTrace = null, string? url = null, CancellationToken ct = default);
}

// Services/ExceptionLogService.cs
public class ExceptionLogService : IExceptionLogService
{
    private readonly IRepositoryBase _repository;
    
    public async Task LogExceptionAsync(Exception exception, string? url = null, CancellationToken ct = default)
    {
        await LogExceptionAsync(exception.Message, exception.StackTrace, url, ct);
    }
    
    public async Task LogExceptionAsync(string message, string? stackTrace = null, string? url = null, CancellationToken ct = default)
    {
        var log = new ExceptionLog
        {
            Message = message ?? string.Empty,
            StackTrace = stackTrace ?? string.Empty,
            Type = exception?.GetType().FullName ?? "Unknown",
            URL = url ?? string.Empty,
            CreatedOn = DateTime.UtcNow
        };
        
        await _repository.InsertAsync(log, ct);
    }
}
```

**Benefits:**
- Centralized exception tracking
- Uses existing migration
- Database-backed error logs

---

### 4. **Global Exception Handling Middleware**
**Why:** Consistent error handling across ASP.NET Core apps.

**Suggested Implementation:**
```csharp
// Middleware/GlobalExceptionHandlerMiddleware.cs
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IExceptionLogService? _exceptionLogService;
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new GenericResponseDto<object>
        {
            StatusCode = exception switch
            {
                ArgumentException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                KeyNotFoundException => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError
            },
            Message = exception.Message,
            Response = null
        };
        
        _logger.LogError(exception, "Unhandled exception occurred");
        
        if (_exceptionLogService != null)
        {
            await _exceptionLogService.LogExceptionAsync(exception, context.Request.Path);
        }
        
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

// ServiceCollection/ExceptionMiddlewareExtensions.cs
public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
```

**Benefits:**
- Consistent error responses
- Automatic exception logging
- Better error handling in production

---

### 5. **Health Check Extensions**
**Why:** Essential for monitoring and Kubernetes/Docker health checks.

**Suggested Implementation:**
```csharp
// ServiceCollection/HealthCheckExtensions.cs
public static class HealthCheckExtensions
{
    public static IServiceCollection AddDatabaseHealthChecks(
        this IServiceCollection services,
        string connectionString,
        DatabaseType dbType)
    {
        return dbType switch
        {
            DatabaseType.SqlServer => services.AddHealthChecks()
                .AddSqlServer(connectionString),
            DatabaseType.MySql => services.AddHealthChecks()
                .AddMySql(connectionString),
            DatabaseType.PostgreSQL => services.AddHealthChecks()
                .AddNpgSql(connectionString),
            _ => throw new ArgumentException("Unsupported database type")
        };
    }
}
```

**Benefits:**
- Standard health check endpoints
- Easy integration with monitoring tools
- Kubernetes readiness/liveness probes

---

## 📦 Medium-Priority Additions

### 6. **Caching Service Interface & Implementation**
**Why:** Performance optimization and reduced database load.

**Suggested Implementation:**
```csharp
// Interfaces/ICacheService.cs
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken ct = default);
    Task ClearAsync(CancellationToken ct = default);
}

// Services/MemoryCacheService.cs - Simple in-memory implementation
// Services/DistributedCacheService.cs - Wrapper for IDistributedCache
```

**Benefits:**
- Standardized caching interface
- Easy to swap implementations
- Works with memory cache or Redis

---

### 7. **Email Service**
**Why:** Most applications need email functionality.

**Suggested Implementation:**
```csharp
// Interfaces/IEmailService.cs
public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken ct = default);
    Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken ct = default);
}

// Services/SmtpEmailService.cs
// Classes/EmailMessage.cs
// Classes/EmailSettings.cs
```

**Benefits:**
- SMTP email wrapper
- Template support (optional)
- Async email sending

---

### 8. **File Upload/Download Helpers**
**Why:** Common requirement for file operations.

**Suggested Implementation:**
```csharp
// Helpers/FileHelper.cs
public static class FileHelper
{
    public static bool IsValidFileExtension(string fileName, string[] allowedExtensions);
    public static bool IsValidFileSize(long fileSize, long maxSizeBytes);
    public static string GetSafeFileName(string fileName);
    public static string GetFileExtension(string fileName);
    public static string GetMimeType(string fileName);
}

// Services/FileStorageService.cs
public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string? folder = null);
    Task<Stream?> GetFileAsync(string filePath);
    Task<bool> DeleteFileAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
}
```

**Benefits:**
- File validation
- Safe file storage
- MIME type detection

---

### 9. **Validation Helpers**
**Why:** Reduce boilerplate validation code.

**Suggested Implementation:**
```csharp
// Helpers/ValidationHelper.cs
public static class ValidationHelper
{
    public static bool IsValidEmail(string? email);
    public static bool IsValidUrl(string? url);
    public static bool IsValidPhoneNumber(string? phoneNumber);
    public static bool IsValidCreditCard(string? cardNumber);
    public static bool IsStrongPassword(string? password, int minLength = 8);
    public static ValidationResult Validate(object obj);
}
```

**Benefits:**
- Common validation rules
- Reusable across projects
- Can integrate with FluentValidation

---

### 10. **HttpClient Factory Extensions**
**Why:** Better HttpClient management and named clients.

**Suggested Implementation:**
```csharp
// ServiceCollection/HttpClientExtensions.cs
public static class HttpClientExtensions
{
    public static IServiceCollection AddNamedHttpClient<T>(
        this IServiceCollection services,
        string clientName,
        Action<HttpClient>? configureClient = null)
    {
        return services.AddHttpClient<T>(clientName, configureClient);
    }
    
    public static IServiceCollection AddApiClient<T>(
        this IServiceCollection services,
        string baseAddress,
        Action<HttpClient>? configureClient = null)
        where T : class
    {
        services.AddHttpClient<T>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            configureClient?.Invoke(client);
        });
        return services;
    }
}
```

**Benefits:**
- Proper HttpClient lifecycle management
- Named clients support
- Easier API client setup

---

### 11. **Correlation ID Middleware**
**Why:** Request tracing across services.

**Suggested Implementation:**
```csharp
// Middleware/CorrelationIdMiddleware.cs
public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    private readonly RequestDelegate _next;
    
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString();
            
        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;
        
        await _next(context);
    }
}

// Helpers/CorrelationIdHelper.cs
public static class CorrelationIdHelper
{
    public static string? GetCorrelationId(HttpContext? context)
    {
        return context?.Items["CorrelationId"]?.ToString();
    }
}
```

**Benefits:**
- Request tracking
- Distributed tracing support
- Debugging aid

---

### 12. **Request/Response Logging Middleware**
**Why:** API monitoring and debugging.

**Suggested Implementation:**
```csharp
// Middleware/RequestResponseLoggingMiddleware.cs
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        // Log request
        _logger.LogInformation("Request: {Method} {Path}", 
            context.Request.Method, context.Request.Path);
        
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        await _next(context);
        
        // Log response
        _logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
        
        await responseBody.CopyToAsync(originalBodyStream);
    }
}
```

**Benefits:**
- Request/response monitoring
- Debugging assistance
- Performance tracking

---

## 🔧 Low-Priority / Nice-to-Have

### 13. **Background Job Helpers**
- Simple background service base class
- Queue-based job processing
- Scheduled job helpers

### 14. **Rate Limiting Middleware**
- Per-IP rate limiting
- Token-based rate limiting
- Configurable limits

### 15. **Configuration Validation Extensions**
- Settings validation on startup
- Options pattern validation helpers
- Configuration health checks

### 16. **Localization Helpers**
- Resource file helpers
- Culture helpers
- Translation utilities

### 17. **Performance Monitoring Helpers**
- Operation timing helpers
- Memory profiling helpers
- Performance counters

### 18. **Testing Helpers**
- Test fixture base classes
- Database seeding helpers
- Mock data generators

### 19. **GraphQL Support (if needed)**
- GraphQL query helpers
- Schema generation helpers

### 20. **SignalR Helpers**
- Hub base classes
- Connection management helpers

---

## 📊 Implementation Priority Recommendation

### Phase 1 (High Impact, Low Effort)
1. ✅ **Pagination Support** - Essential, easy to implement
2. ✅ **Result<T> Pattern** - Better error handling
3. ✅ **Exception Logging Service** - Uses existing migration
4. ✅ **Global Exception Middleware** - Critical for APIs

### Phase 2 (High Impact, Medium Effort)
5. ✅ **Health Check Extensions** - Monitoring essential
6. ✅ **Caching Service** - Performance boost
7. ✅ **Email Service** - Common requirement
8. ✅ **File Upload Helpers** - Common requirement

### Phase 3 (Medium Impact)
9. ✅ **Validation Helpers** - Reduces boilerplate
10. ✅ **HttpClient Factory Extensions** - Better practices
11. ✅ **Correlation ID Middleware** - Tracing support
12. ✅ **Request/Response Logging** - Debugging aid

---

## 💡 Additional Considerations

1. **Keep it focused:** Don't add everything. Choose features that are:
   - Commonly needed across projects
   - Not well-covered by existing libraries
   - Complementary to existing features

2. **Dependency management:** Consider if new features require additional NuGet packages and their impact on the library size.

3. **Backward compatibility:** Ensure new features don't break existing code.

4. **Documentation:** Every new feature needs XML documentation and README updates.

5. **Testing:** Consider adding a test project to validate new features.

---

## 🎯 Recommended Starting Points

Based on the library's current state, I recommend starting with:

1. **Pagination Support** - Most requested, easy to implement, high impact
2. **Global Exception Middleware** - Complements existing exception logging migration
3. **Result<T> Pattern** - Modern error handling approach
4. **Exception Logging Service** - Completes the exception logging feature

These four additions would significantly enhance the library's value with minimal complexity.

