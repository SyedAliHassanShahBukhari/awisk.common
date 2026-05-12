# awisk.common — Improvement Proposals

Ten focused changes identified from code review. Each has its own document with the problem, proposed code, and impact.

---

## Index

| # | Document | Area | Type | Breaking? | Priority |
|---|----------|------|------|-----------|----------|
| 01 | [Result<T>: Add Map, Bind, Match](01-result-functional-operators.md) | Common | Feature | No | Medium |
| 02 | [Repository: Eliminate extra round trips in Exists/Delete](02-repository-exists-delete-round-trips.md) | Data | Performance | No | High |
| 03 | [Repository: Fix O(n²) CreateBatches](03-createbatches-on2-fix.md) | Data | Performance | No | High |
| 04 | [Middleware: Cache JsonSerializerOptions](04-middleware-json-options-static.md) | Middleware | Performance | No | Low |
| 05 | [ApiService: Fix silent failure in PutAsync](05-apiservice-putasync-silent-fail.md) | Services | Bug Fix | Yes* | Critical |
| 06 | [BaseEntity: Replace sentinel dates with nullable](06-baseentity-nullable-dates.md) | Data | Design | Yes | Medium |
| 07 | [EnumHelper: Cache reflection results](07-enumhelper-reflection-cache.md) | Helpers | Performance | No | Low |
| 08 | [TokenService: Remove redundant Token claim](08-tokenservice-remove-token-claim.md) | Services | Security | Yes* | High |
| 09 | [PagedResponse: Guard divide-by-zero in TotalPages](09-pagedresponse-divide-by-zero.md) | DTOs | Bug Fix | No | Critical |
| 10 | [Repository: Add transaction support](10-repository-transaction-support.md) | Data | Feature | No | High |

\* Breaking only for consuming projects that rely on the current (incorrect) behaviour.

---

## Recommended Implementation Order

1. **#09** — PagedResponse divide-by-zero (one-line fix, zero risk)
2. **#04** — Middleware JsonSerializerOptions (one-line fix, zero risk)
3. **#05** — ApiService PutAsync silent fail (bug fix, confirm no consumers rely on silent return)
4. **#03** — CreateBatches O(n²) (swap to `Chunk`, low risk)
5. **#07** — EnumHelper reflection cache (additive change, no risk)
6. **#02** — Repository Exists/Delete round trips (requires table name helper)
7. **#08** — Remove Token claim (confirm no consumers read `"Token"` claim)
8. **#01** — Result Map/Bind/Match (additive, review async variants as follow-up)
9. **#10** — Repository transaction support (additive, test with all three DB providers)
10. **#06** — BaseEntity nullable dates (requires DB migrations in all consuming projects — plan separately)
