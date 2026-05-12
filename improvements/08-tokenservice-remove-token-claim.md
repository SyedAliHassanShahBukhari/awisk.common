# Change 08 — TokenService: Remove Redundant "Token" Claim from JWT

## File
`awisk.common/Services/TokenService.cs` (line 96)

## Problem
`CreateClaims` embeds the existing JWT string as a custom claim inside the new JWT being constructed:

```csharp
public List<Claim> CreateClaims(TokenResponseDto response)
{
    List<Claim> claims = [
        new Claim(JwtRegisteredClaimNames.Email, ...),
        new Claim(JwtRegisteredClaimNames.Jti, ...),
        new Claim(ClaimTypes.NameIdentifier, ...),
        new Claim(ClaimTypes.Name, ...),
        new Claim("Token", UniversalOpertaions.IfNullEmptyString(response.Token))  // <-- problem
    ];
    // ...
}
```

This has two issues:

### Issue 1 — Circular / meaningless data
`CreateClaims` is called from `GenerateJwtTokenStr(TokenResponseDto)`, which then calls `GetJwtHandler` to build the token. At the point `CreateClaims` runs, `response.Token` is the *previous* token (or whatever was stored in the DTO), not the token being generated. The resulting JWT contains a stale or unrelated token string inside its own claims payload.

### Issue 2 — Security risk
Embedding a full JWT string inside another JWT's payload:
- Inflates the token size unnecessarily (JWTs are typically sent in every HTTP request header).
- The embedded token string may itself be a valid, unexpired token. If the outer token is decoded (JWTs are base64, not encrypted by default), the inner token is exposed in plain text in any log or debugging tool that decodes the JWT.

## Proposed Change
Remove the `"Token"` claim entirely:

```csharp
public List<Claim> CreateClaims(TokenResponseDto response)
{
    List<Claim> claims = [
        new Claim(JwtRegisteredClaimNames.Email, UniversalOpertaions.IfNullEmptyString(response.Email)),
        new Claim(JwtRegisteredClaimNames.Jti, UniversalOpertaions.NewGuidStr()),
        new Claim(ClaimTypes.NameIdentifier, UniversalOpertaions.IfNullEmptyString(response.Id)),
        new Claim(ClaimTypes.Name, UniversalOpertaions.IfNullEmptyString(response.FullName))
    ];
    // role claims unchanged...
    return claims;
}
```

## Impact
- **Smaller tokens** — removes one claim worth potentially hundreds of characters.
- **No functional impact** — no legitimate use case requires reading a token string from inside its own claims. The token is available to clients directly from the `TokenResponseDto.Token` field returned by the login endpoint.
- If any consuming project reads `User.FindFirst("Token")` it will stop finding this claim — those callers should be updated to use the token returned at login time.
