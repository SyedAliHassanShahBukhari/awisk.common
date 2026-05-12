# Change 02 — Repository: Eliminate Extra Round Trips in Exists and Delete<T,ID>

## Files
- `awisk.common/Data/Db/RepositoryBaseSqlServer.cs` (lines 53–60, 382–392)
- Same pattern applies to `RepositoryBaseMySql.cs` and `RepositoryBasePostgreSql.cs`

## Problem
Two methods each make two database round trips where one would suffice.

### `Delete<T, ID>` (line 53)
```csharp
public bool Delete<T, ID>(ID id) where T : class
{
    T? byId = GetById<T, ID>(id);   // Round trip 1: SELECT full entity
    if (byId == null) return false;
    return Delete(byId);            // Round trip 2: DELETE
}
```
Fetches the entire entity just to pass it into the `Delete(T item)` overload. Dapper.Contrib requires an entity object for its `Delete<T>`, but a direct SQL DELETE by ID needs only one trip.

### `Exists<T, ID>` (line 382)
```csharp
public bool Exists<T, ID>(ID id) where T : class
{
    T? entity = GetById<T, ID>(id); // Round trip 1: SELECT full entity + all columns
    return entity != null;          // only null check needed
}
```
Loads every column of the row to answer a yes/no question.

## Proposed Change

### `Exists<T, ID>` — single scalar query
```csharp
public bool Exists<T, ID>(ID id) where T : class
{
    var tableName = typeof(T).Name;
    using SqlConnection connection = new(ConnectionString);
    return connection.QuerySingle<int>(
        $"SELECT COUNT(1) FROM [{tableName}] WHERE Id = @Id",
        new { Id = id }) > 0;
}

public async Task<bool> ExistsAsync<T, ID>(ID id) where T : class
{
    var tableName = typeof(T).Name;
    using SqlConnection connection = new(ConnectionString);
    return await connection.QuerySingleAsync<int>(
        $"SELECT COUNT(1) FROM [{tableName}] WHERE Id = @Id",
        new { Id = id }).ConfigureAwait(false) > 0;
}
```

### `Delete<T, ID>` — single DELETE statement
```csharp
public bool Delete<T, ID>(ID id) where T : class
{
    var tableName = typeof(T).Name;
    using SqlConnection connection = new(ConnectionString);
    var affected = connection.Execute(
        $"DELETE FROM [{tableName}] WHERE Id = @Id",
        new { Id = id });
    return affected > 0;
}

public async Task<bool> DeleteAsync<T, ID>(ID id) where T : class
{
    var tableName = typeof(T).Name;
    using SqlConnection connection = new(ConnectionString);
    var affected = await connection.ExecuteAsync(
        $"DELETE FROM [{tableName}] WHERE Id = @Id",
        new { Id = id }).ConfigureAwait(false);
    return affected > 0;
}
```

## Impact
- **Performance:** Cuts two database calls to one per operation. On high-throughput services this halves the DB load for these methods.
- **No breaking changes:** Method signatures are identical.
- **Caveat:** The table name is derived from `typeof(T).Name`. If any entity class name differs from the actual table name (via Dapper.Contrib `[Table]` attribute), this will break. A helper that reads the `[Table]` attribute with a fallback to the class name should be added.

## Table Name Helper (recommended prerequisite)
```csharp
private static string GetTableName<T>()
{
    var attr = typeof(T).GetCustomAttribute<TableAttribute>();
    return attr?.Name ?? typeof(T).Name;
}
```
