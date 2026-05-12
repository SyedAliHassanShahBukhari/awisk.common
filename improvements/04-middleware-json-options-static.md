# Change 04 — GlobalExceptionHandlerMiddleware: Cache JsonSerializerOptions

## File
`awisk.common/Middleware/GlobalExceptionHandlerMiddleware.cs` (lines 105–109)

## Problem
A new `JsonSerializerOptions` instance is constructed inside `HandleExceptionAsync` — which means it is created every time an exception is caught:

```csharp
private async Task HandleExceptionAsync(HttpContext context, Exception exception)
{
    // ... (called on every unhandled exception)

    var jsonOptions = new JsonSerializerOptions   // <-- allocated per exception
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    var json = JsonSerializer.Serialize(response, jsonOptions);
    // ...
}
```

`JsonSerializerOptions` is a relatively expensive object to construct. The .NET docs explicitly recommend reusing a single instance. While exceptions are not the hot path, allocating it here is unnecessary work, and the middleware pattern encourages treating the instance as a singleton.

## Proposed Change
Promote `jsonOptions` to a `private static readonly` field:

```csharp
public class GlobalExceptionHandlerMiddleware
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    // ... existing fields and constructor unchanged

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // ... same logic, just use _jsonOptions instead of local variable
        var json = JsonSerializer.Serialize(response, _jsonOptions);
        await context.Response.WriteAsync(json).ConfigureAwait(false);
    }
}
```

## Impact
- **No breaking changes** — purely internal implementation detail.
- Eliminates an allocation and initialization cost on every exception.
- Follows the Microsoft-recommended pattern for `JsonSerializerOptions` reuse.
