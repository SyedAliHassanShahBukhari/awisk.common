# Change 05 — ApiService: Fix Silent Failure in PutAsync

## File
`awisk.common/Services/ApiService.cs` (lines 59–89)

## Problem
The four HTTP methods in `ApiService` have inconsistent error handling on non-2xx responses:

| Method | Behaviour on non-2xx |
|--------|----------------------|
| `GetAsync` | Throws via `EnsureSuccessStatusCode()` |
| `PostAsync` | Throws via `EnsureSuccessStatusCode()` |
| `PutAsync` | **Returns `new TResponse()` silently** |
| `DeleteAsync` | Returns `false` (acceptable for delete) |
| `PostWithFileAsync` | Throws via `EnsureSuccessStatusCode()` |

```csharp
// PutAsync — current
var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
if (!response.IsSuccessStatusCode)
{
    return new();   // <-- silent empty object returned, caller has no idea it failed
}
```

A caller that posts data and receives a silently empty `TResponse` will either silently corrupt state or throw a NullReferenceException later when it tries to use the response. This is harder to debug than an immediate exception at the HTTP boundary.

## Proposed Change
Make `PutAsync` consistent with `GetAsync` and `PostAsync` — throw on non-2xx:

```csharp
public async Task<TResponse?> PutAsync<TRequest, TResponse>(
    Uri url, TRequest data, string? bearerToken = null, Dictionary<string, string>? headers = null)
    where TResponse : class, new()
{
    using var request = new HttpRequestMessage(HttpMethod.Put, url)
    {
        Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
    };

    if (!string.IsNullOrEmpty(bearerToken))
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

    if (headers != null)
        foreach (var header in headers)
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);

    var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
    response.EnsureSuccessStatusCode();  // consistent with Get/Post

    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
}
```

## Impact
- **Behaviour change:** Callers that currently rely on the silent `new TResponse()` return will now receive an `HttpRequestException` instead. This is the correct behaviour.
- **No signature change** — return type and parameters are unchanged.
- Callers that already wrap the call in try/catch (for `Get`/`Post`) will handle `Put` failures the same way.

## Recommendation
Also remove the `where TResponse : class, new()` constraint from `PutAsync`. It was only required to support `return new()`. Without the silent-return path, the constraint is unnecessary and inconsistent with the other methods.
