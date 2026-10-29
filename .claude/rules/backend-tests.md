---
paths:
  - "BackendApi.Tests/**"
---
# Backend tests

- `BackendApi.Tests/` (repo root, xUnit): integration tests through HTTP against a real PostGIS container (Testcontainers, `postgis/postgis:18-3.6`).
- `Infrastructure/ApiFixture` starts one container + `ApiFactory` (WebApplicationFactory) per run; schema via `Migrate` (the full migration chain). Respawn resets data before each test; seed tables are kept.
- `ApiFactory` uses the `Testing` environment (no user-secrets), a test JWT key, the container connection string, `FakeOsrmService` and `FakeOverpassClient`.
- New test classes inherit `ApiTestBase` (`CreateUserAsync(roleIds, directPermissions)` returns a logged-in client).
- Run: see root CLAUDE.md Commands.
