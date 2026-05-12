# Change 09 — PagedResponse: Guard Against Divide-by-Zero in TotalPages

## File
`awisk.common/DTOs/Responses/PagedResponse.cs` (line 32)

## Problem
`TotalPages` divides by `PageSize` without checking if `PageSize` is zero:

```csharp
public int TotalPages => TotalCount > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
```

If `PageSize` is `0` and `TotalCount` is greater than `0`, this expression evaluates `TotalCount / 0.0` which in C# returns `double.PositiveInfinity`. Casting infinity to `int` returns `int.MinValue` (-2,147,483,648) — not an exception, but silently wrong data that will cause unexpected behaviour in any pagination UI or API consumer.

A `PageSize` of zero is possible when:
- A misconfigured client sends `pageSize=0`.
- `PagedResponse` is constructed directly with `pageSize: 0` in code.
- Default constructor is used and `PageSize` is never set (defaults to `0`).

## Proposed Change
Add a `PageSize > 0` guard to the condition:

```csharp
public int TotalPages => (TotalCount > 0 && PageSize > 0)
    ? (int)Math.Ceiling(TotalCount / (double)PageSize)
    : 0;
```

## Additional Recommendation
Add validation in the `PagedRequest` class to reject `PageSize <= 0` at the boundary (before it reaches repository calls). This catches the problem at the source rather than relying on `PagedResponse` to handle it:

```csharp
// In PagedRequest.cs
public int PageSize
{
    get => _pageSize;
    set => _pageSize = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(PageSize), "PageSize must be greater than zero.");
}
private int _pageSize = 10; // sensible default
```

## Impact
- **No breaking changes** — the fix only changes the result from `int.MinValue` (currently wrong) to `0` (correct).
- Prevents silent corrupted pagination metadata reaching API consumers.
