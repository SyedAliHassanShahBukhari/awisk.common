# Change 01 — Result<T>: Add Functional Operators (Map, Bind, Match)

## File
`awisk.common/Common/Result.cs`

## Problem
The current `Result<T>` type is a plain container. There are no chaining methods, so callers must manually unwrap and check `IsSuccess` every time they want to transform or compose results. This leads to repetitive boilerplate:

```csharp
// Current — every caller writes this pattern
var result = GetUser(id);
if (!result.IsSuccess)
    return Result<UserDto>.Failure(result.ErrorMessage!);

var dto = MapToDto(result.Value!);
return Result<UserDto>.Success(dto);
```

## Proposed Change
Add three operators to `Result<T>`:

| Method | Purpose |
|--------|---------|
| `Map<TNew>(Func<T, TNew>)` | Transform the value on success, pass failure through |
| `Bind<TNew>(Func<T, Result<TNew>>)` | Chain to another Result-returning operation |
| `Match<TOut>(onSuccess, onFailure)` | Collapse the result into a single value (useful in controllers) |

```csharp
// Map — transforms the value if successful
public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
{
    ArgumentNullException.ThrowIfNull(mapper);
    return IsSuccess ? Result<TNew>.Success(mapper(Value!)) : Result<TNew>.Failure(ErrorMessage!, Exception);
}

// Bind — chains to another operation that also returns a Result
public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder)
{
    ArgumentNullException.ThrowIfNull(binder);
    return IsSuccess ? binder(Value!) : Result<TNew>.Failure(ErrorMessage!, Exception);
}

// Match — converts Result into any value (no more if/else in controllers)
public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<string, TOut> onFailure)
{
    ArgumentNullException.ThrowIfNull(onSuccess);
    ArgumentNullException.ThrowIfNull(onFailure);
    return IsSuccess ? onSuccess(Value!) : onFailure(ErrorMessage ?? string.Empty);
}
```

### Usage after change
```csharp
// Chain operations cleanly
var result = GetUser(id)
    .Map(user => MapToDto(user))
    .Bind(dto => ValidateDto(dto));

// Collapse in a controller action
return result.Match(
    onSuccess: dto => Ok(dto),
    onFailure: err => BadRequest(err)
);
```

## Impact
- **No breaking changes** — existing code using `IsSuccess`/`Value`/`ErrorMessage` is unaffected.
- Reduces boilerplate at every call site that chains results.
- Makes controller actions significantly cleaner.

## Notes
- `Failure(ErrorMessage!, Exception)` — the `Failure(string, Exception?)` overload already exists so passing a potentially-null Exception is fine; add a guard inside if needed.
- Async variants (`MapAsync`, `BindAsync`) can be added in a follow-up once the sync versions are proven.
