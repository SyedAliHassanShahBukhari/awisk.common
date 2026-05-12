# Change 03 — Repository: Fix O(n²) CreateBatches Implementation

## File
`awisk.common/Data/Db/RepositoryBaseSqlServer.cs` (lines 184–204)

## Problem
The current `CreateBatches<T>` uses `List.RemoveRange(0, count)` in a loop:

```csharp
var tempItems = items.ToList();
var batches = new List<List<T>>();

while (tempItems.Count > 0)
{
    var batch = tempItems.Take(batchSize).ToList();
    batches.Add(batch);
    tempItems.RemoveRange(0, batch.Count); // <-- shifts entire array each iteration
}
```

`RemoveRange(0, n)` on a `List<T>` copies all remaining elements one position left. For a list of 100,000 items with batch size 2,000, this runs 50 iterations and shifts ~5 million elements in total — O(n²) overall.

It also builds a `List<List<T>>` eagerly in memory before returning, holding two full copies of the data at peak (original + batches).

## Proposed Change

### Option A — Use the .NET 6+ built-in `Chunk`
```csharp
public static IEnumerable<IEnumerable<T>> CreateBatches<T>(
    IEnumerable<T> items, int batchSize = DefaultBatchSize)
{
    ArgumentNullException.ThrowIfNull(items);
    if (batchSize <= 0)
        throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be greater than zero.");

    return items.Chunk(batchSize);
}
```
`Enumerable.Chunk` is O(n), lazy, and already tested by the framework. Since this library targets .NET 9 this is the preferred approach.

### Option B — Index-based loop (if Chunk is not desired)
```csharp
public static IEnumerable<IEnumerable<T>> CreateBatches<T>(
    IEnumerable<T> items, int batchSize = DefaultBatchSize)
{
    ArgumentNullException.ThrowIfNull(items);
    if (batchSize <= 0)
        throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be greater than zero.");

    var list = items as IList<T> ?? items.ToList();
    for (int i = 0; i < list.Count; i += batchSize)
        yield return list.Skip(i).Take(batchSize);
}
```

## Impact
- **Performance:** O(n) instead of O(n²). For 100k items with batch size 2k the difference is ~50 array-copy operations vs ~5M element shifts.
- **Memory:** Option A (Chunk) is lazy — no intermediate list of lists allocated.
- **No breaking changes:** Method signature is identical; return type `IEnumerable<IEnumerable<T>>` is unchanged.

## Recommendation
Use Option A (`Chunk`). It is one line, correct, and already optimised by the runtime.
