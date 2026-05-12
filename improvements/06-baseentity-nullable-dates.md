# Change 06 — BaseEntity: Replace Sentinel Dates with Nullable DateTime

## File
`awisk.common/Data/Db/BaseEntity.cs` (lines 17–18)

## Problem
`UpdatedOn` and `DeletedOn` use `new DateTime(1900, 01, 01)` as a sentinel value meaning "not yet set":

```csharp
public DateTime UpdatedOn { get; set; } = new(1900, 01, 01);
public DateTime DeletedOn { get; set; } = new(1900, 01, 01);
```

Problems with this approach:

1. **Semantically wrong** — a date of 1900-01-01 has no real meaning. `null` is the correct way to express "this has never happened."
2. **Leaks into queries** — `WHERE UpdatedOn > @SomeDate` will unexpectedly include unmodified rows because 1900-01-01 is a valid date.
3. **Leaks into API responses** — serialising these fields returns `"1900-01-01T00:00:00"` to API consumers rather than `null`.
4. **Inconsistency with `DeletedBy` / `UpdatedBy`** — those string fields default to `string.Empty` which also indicates "not set", but at least empty string does not mislead date arithmetic.

## Proposed Change
Change both fields to `DateTime?`:

```csharp
public abstract class BaseEntity<T>
{
    public T Id { get; set; } = default!;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedOn { get; set; }          // null = never updated
    public string DeletedBy { get; set; } = string.Empty;
    public DateTime? DeletedOn { get; set; }          // null = not deleted
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
}
```

The same change applies to `BaseGuidEntity`, `BaseIntEntity`, and `BaseLongEntity` if they duplicate these fields.

## Migration Impact
Existing database columns for `UpdatedOn` and `DeletedOn` must be changed from `DATETIME NOT NULL DEFAULT '1900-01-01'` to `DATETIME NULL`. A migration script is required:

```sql
-- SQL Server
ALTER TABLE [YourTable] ALTER COLUMN UpdatedOn DATETIME NULL;
ALTER TABLE [YourTable] ALTER COLUMN DeletedOn DATETIME NULL;

-- Update existing 1900 sentinel values to NULL
UPDATE [YourTable] SET UpdatedOn = NULL WHERE UpdatedOn = '1900-01-01';
UPDATE [YourTable] SET DeletedOn = NULL WHERE DeletedOn = '1900-01-01';
```

## Impact
- **Breaking change** in consuming projects that compare these fields to `new DateTime(1900, 01, 01)` — those checks should be replaced with `== null`.
- Fixes incorrect query results involving date comparisons.
- Fixes `1900-01-01T00:00:00` appearing in API responses.
- Recommended to ship with a documented migration guide.
