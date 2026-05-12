# Change 07 — EnumHelper: Cache Reflection Results

## File
`awisk.common/Helpers/EnumHelper.cs` (lines 30–36)

## Problem
`GetEnumDescription` calls reflection on every invocation:

```csharp
private static string GetEnumDescription<T>(T enumValue) where T : Enum
{
    var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());   // reflection
    var descriptionAttribute = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false)
        .FirstOrDefault() as DescriptionAttribute;                        // reflection
    return descriptionAttribute?.Description ?? enumValue.ToString();
}
```

This is called from `GetSelectListFromEnum<T>`, `ToTupleList<T>`, `ToDictionary<T>`, `ToJsonList<T>`, and `ToDescription(this Enum)`. In any scenario that renders a dropdown or serialises a list, each enum value hits reflection every time.

`GetField` + `GetCustomAttributes` are among the more expensive reflection operations. Enum values are fixed at compile time — their descriptions never change at runtime. This is an ideal cache candidate.

## Proposed Change
Add a static `ConcurrentDictionary` cache keyed by `(Type, string)`:

```csharp
private static readonly ConcurrentDictionary<(Type EnumType, string MemberName), string> _descriptionCache = new();

private static string GetEnumDescription<T>(T enumValue) where T : Enum
{
    var key = (typeof(T), enumValue.ToString());
    return _descriptionCache.GetOrAdd(key, static k =>
    {
        var field = k.EnumType.GetField(k.MemberName);
        return field?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? k.MemberName;
    });
}
```

Apply the same cache to `ToDescription(this Enum)`:

```csharp
public static string ToDescription(this Enum enumValue)
{
    ArgumentNullException.ThrowIfNull(enumValue);
    var key = (enumValue.GetType(), enumValue.ToString());
    return _descriptionCache.GetOrAdd(key, static k =>
    {
        var field = k.EnumType.GetField(k.MemberName);
        return field?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? k.MemberName;
    });
}
```

## Impact
- **Performance:** Reflection runs once per unique enum member across the application lifetime. All subsequent calls return the cached string.
- **No breaking changes** — method signatures and return values are identical.
- `ConcurrentDictionary` is thread-safe and suitable for static use.
- Memory cost is negligible — most applications have fewer than a few hundred total enum members.
