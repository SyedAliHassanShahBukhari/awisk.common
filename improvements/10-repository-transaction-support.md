# Change 10 — Repository: Add Transaction Support

## Files
- `awisk.common/Data/Db/Interfaces/IRepositoryBase.cs`
- `awisk.common/Data/Db/RepositoryBaseSqlServer.cs`
- `awisk.common/Data/Db/RepositoryBaseMySql.cs`
- `awisk.common/Data/Db/RepositoryBasePostgreSql.cs`

## Problem
The repository has no transaction support. Every method opens its own connection, executes, and closes. Multi-step write operations (e.g., insert order + insert order lines + update inventory) cannot be wrapped in a single atomic transaction:

```csharp
// Currently impossible to do atomically with the repository:
repo.Insert(order);          // connection 1 — committed immediately
repo.Insert(orderLines);     // connection 2 — committed immediately
repo.Update(inventory);      // connection 3 — if this fails, order + lines are already in DB
```

Without transactions, any failure mid-operation leaves the database in a partially-written state.

## Proposed Change

### 1. Add `ExecuteInTransactionAsync` to the interface

```csharp
// IRepositoryBase.cs
Task<T> ExecuteInTransactionAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> work);
Task ExecuteInTransactionAsync(Func<IDbConnection, IDbTransaction, Task> work);
```

### 2. Implement in RepositoryBaseSqlServer

```csharp
public async Task<T> ExecuteInTransactionAsync<T>(
    Func<IDbConnection, IDbTransaction, Task<T>> work)
{
    using SqlConnection connection = new(ConnectionString);
    await connection.OpenAsync().ConfigureAwait(false);
    using var transaction = connection.BeginTransaction();
    try
    {
        var result = await work(connection, transaction).ConfigureAwait(false);
        transaction.Commit();
        return result;
    }
    catch
    {
        transaction.Rollback();
        throw;
    }
}

public async Task ExecuteInTransactionAsync(
    Func<IDbConnection, IDbTransaction, Task> work)
{
    using SqlConnection connection = new(ConnectionString);
    await connection.OpenAsync().ConfigureAwait(false);
    using var transaction = connection.BeginTransaction();
    try
    {
        await work(connection, transaction).ConfigureAwait(false);
        transaction.Commit();
    }
    catch
    {
        transaction.Rollback();
        throw;
    }
}
```

### 3. Usage example

```csharp
await repo.ExecuteInTransactionAsync(async (conn, tx) =>
{
    await conn.InsertAsync(order, transaction: tx);
    await conn.InsertAsync(orderLines, transaction: tx);
    await conn.UpdateAsync(inventory, transaction: tx);
});
// All three committed atomically, or all rolled back on failure
```

## Scope
This change adds new methods — it does not modify existing ones. Existing calling code is completely unaffected.

## Notes
- The pattern passes `IDbConnection` and `IDbTransaction` to the delegate so callers can use Dapper directly with transaction support.
- Dapper's `Execute`/`Query`/`Insert`/`Update`/`Delete` methods all accept an optional `transaction` parameter.
- MySQL and PostgreSQL implementations follow the same pattern using `MySqlConnection` and `NpgsqlConnection` respectively.
- A sync overload can be added later if needed, but async-first is preferred for database work.
