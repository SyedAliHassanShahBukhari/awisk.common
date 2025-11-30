# BaseEntity<T> Usage Guide

## Overview

`BaseEntity<T>` is a generic base class that allows you to use different ID types for your entities. However, there are some important considerations and best practices.

## Supported ID Types

### ✅ Recommended Types (Common Database Primary Keys)

1. **`int`** - Auto-increment integer (most common)
   ```csharp
   public class Product : BaseEntity<int>
   {
       [Key]  // Required for Dapper.Contrib
       public override int Id { get; set; }
       // ... other properties
   }
   ```

2. **`long`** - Auto-increment bigint (for large datasets)
   ```csharp
   public class Order : BaseEntity<long>
   {
       [Key]  // Required for Dapper.Contrib
       public override long Id { get; set; }
       // ... other properties
   }
   ```

3. **`Guid`** - Globally unique identifier
   ```csharp
   public class User : BaseEntity<Guid>
   {
       [ExplicitKey]  // Required for Dapper.Contrib (non-auto-increment)
       public override Guid Id { get; set; } = Guid.NewGuid();
       // ... other properties
   }
   ```

4. **`string`** - String-based IDs (e.g., custom codes, slugs)
   ```csharp
   public class Category : BaseEntity<string>
   {
       [ExplicitKey]  // Required for Dapper.Contrib
       public override string Id { get; set; } = string.Empty;
       // ... other properties
   }
   ```

### ⚠️ Not Recommended (But Technically Possible)

5. **`bool`** - **NOT RECOMMENDED** - Only 2 possible values (true/false), impractical for primary keys
6. **`byte`** - Limited to 256 values, rarely used
7. **`short`** - Limited to 65,536 values, rarely used
8. **`decimal`** - Can work but unusual for primary keys
9. **`DateTime`** - Can work but unusual, prefer Guid or int

## Important Notes

### 1. Dapper.Contrib Attributes Required

You **MUST** add the appropriate attribute to your `Id` property:

- **`[Key]`** - For auto-increment IDs (int, long)
- **`[ExplicitKey]`** - For non-auto-increment IDs (Guid, string, custom types)

### 2. Default Values

- **Value types** (int, long, bool): Defaults to `0`, `0`, `false` respectively
- **Reference types** (string, Guid): Defaults to `null` (or `Guid.Empty` for Guid)
- You may want to override the default in your derived class

### 3. Example Implementations

```csharp
// Example 1: Using int (auto-increment)
public class Product : BaseEntity<int>
{
    [Key]
    public override int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// Example 2: Using Guid (explicit key)
public class User : BaseEntity<Guid>
{
    [ExplicitKey]
    public override Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
}

// Example 3: Using string (explicit key)
public class Category : BaseEntity<string>
{
    [ExplicitKey]
    public override string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
```

### 4. Alternative: Use Specific Base Classes

Instead of `BaseEntity<T>`, you can use the provided specific base classes:

- `BaseGuidEntity` - Uses `Guid` with `[ExplicitKey]`
- `BaseIntEntity` - Uses `int` with `[Key]`
- `BaseLongEntity` - Uses `long` with `[Key]`

These are simpler and don't require you to specify the generic type:

```csharp
// Simpler - no generic type needed
public class Product : BaseIntEntity
{
    // Id is already defined as int with [Key]
    public string Name { get; set; } = string.Empty;
}

// Using Guid-based entity
public class User : BaseGuidEntity
{
    // Id is already defined as Guid with [ExplicitKey]
    public string Email { get; set; } = string.Empty;
}
```

## Best Practices

1. **Use `BaseEntity<T>` when** you need flexibility or custom ID types
2. **Use specific base classes** (`BaseIntEntity`, `BaseLongEntity`) when you have a standard ID type
3. **Always add `[Key]` or `[ExplicitKey]`** attribute to your Id property
4. **Avoid `bool` as ID type** - it's impractical (only 2 values)
5. **Consider database compatibility** - Ensure your database supports the ID type you choose

## Summary

**Yes, `BaseEntity<T>` can technically be used with any datatype**, but:
- ✅ **Recommended**: int, long, Guid, string
- ⚠️ **Possible but unusual**: byte, short, decimal, DateTime
- ❌ **Not recommended**: bool (only 2 values)

The most common and practical choices are **int**, **long**, **Guid**, and **string**.

