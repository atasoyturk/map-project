---
paths:
  - "BackendApi/**"
---
# Backend rules

## Architecture
Controller → Service (interface + implementation, registered in `Program.cs`) → `AppDbContext` (EF Core + NetTopologySuite).
- Exception: Auth uses `Repositories/` (`IUserRepository` / `UserRepository`) instead of `AppDbContext`; `AuthController` calls it directly (known).
- Folders are grouped by domain: `Controllers/<Domain>`, `Services/<Domain>`, `DTOs/<Domain>`, `Entities/<Domain>`.
  Controller folder = service folder = DTO folder = namespace (per domain). Routes keep their existing names.
  Using another domain's DTOs or services is fine; the rule is about where a file lives.
  Exceptions (existing, keep as is):
  - Routes named differently from their controller: `Geo/SearchController` → `api/drawings/search`,
    `Analysis/ThematicMapController` → `api/thematic-map`,
    `Geo/GeoController` → `api/point|line|polygon` (one controller over `PointService`/`LineService`/`PolygonService`).
  - `Services/Permission` + `DTOs/Permission` have no controller; geo permissions live in `Geo`.
  - `Entities/Common` (`BaseEntity`) and `Entities/Team` have no matching controller/service folder.
- Controllers stay thin: read user info, call the service, map the result to an HTTP response. No business logic or DB access.
- Controllers inherit `ApiControllerBase` (`[Authorize]`, `GetUserId()`, `GetTeamId()`, `GetUserRoles()`, `HasAdminAccess()`).
  Exception: `AuthController` (anonymous login/register) inherits `ControllerBase`.
- Request/response shapes are DTOs (`sealed record` or `sealed class`). Never return entities from controllers.
- New service → interface `IXService` + `XService`, register with `AddScoped` in `Program.cs`.
  Exception: `RouteSimulationService` is a singleton; it must use `IServiceScopeFactory` for DB access.
- **Soft delete only**: use `entity.SoftDelete(userId)` (`Helpers/EntityExtensions.cs`). Every query filters `!IsDeleted`.
  Exceptions: see `.claude/rules/migrations-seed.md` (link tables, `Role`).
- Geometry: always parse WKT with `GeometryConverter.FromWkt` (10,000-point limit, rejects invalid geometries) and
  output with `GeometryConverter.ToWkt`. SRID 4326 (lon, lat).

## Authorization
- Endpoint permissions: `[RequirePermission("permission_name")]`.
  A new permission needs: seed entry in `Data/Seed/`, policy in `Program.cs`, and role mapping — **ask first**.
- New code must authorize by permission (or `admin_access`), never by role name — the role set will be simplified (see `planning/vision.md`).
- JWT carries the user's effective permissions as `permission` claims, plus a separate `admin_access` claim
  (`"true"`) for admins; built in `JwtTokenService`, issued at login.
  Permission changes take effect after the user logs in again.
- Geographic limits: create/update of drawn features, POIs, stops and annotations must check
  `IGeoPermissionService.IsWithinBoundaryAsync` / `EnsureWithinBoundaryAsync`.
  Admins (`admin_access`, passed as `HasAdminAccess()`) are exempt; the exemption lives only in `GeoPermissionService` — do not add
  admin checks at call sites. A boundary cannot be assigned to an admin user or to a role with `admin_access` (403).
- Data visibility uses `GeoViewMode` (Own / Team / All) resolved by `GeoViewModeResolver`.
  Visibility filtering is not global: every endpoint that returns drawings must apply `GeoViewModeResolver`
  (or the same filter) itself.
- GeoServer is only reached through `GeoServerService`; layers must be in the `AllowedLayers` whitelist. User-data layers get a CQL filter; shared layers (`tbl_poi_active`, `tbl_transit_stop_active`) are unfiltered on purpose:
  POIs (mostly OSM-imported) and stops are shared reference data every user may see. Do not add a CQL filter for them.
- OSM POI import (`POST api/poi/import-osm`, `admin_access`) skips the geo-boundary check: admins are exempt anyway.

## Errors
- Throw, don't return error codes from services. `GlobalExceptionHandler` maps:
  `ArgumentException`→400, `UnauthorizedAccessException`→403, `KeyNotFoundException`→404, `InvalidOperationException`→409, others→500.
- Return `null` / `false` from services for "not found", and let the controller return `NotFound()`.

## Config & secrets
- Settings classes in `Settings/` with DataAnnotations + `ValidateOnStart`.
- Secrets (JWT key, connection string, GeoServer credentials) live in **user-secrets**. Never write secrets into `appsettings.json` or code.

## Key algorithms (read before changing)
- `TripPlanningService`: graph of walk/bus/transfer edges + Dijkstra, Haversine distances.
- `ThematicMapService`: isochrone with 16 OSRM rays + interpolation.
- `RouteSimulationService`: time-scaled simulation, 250 ms tick, SignalR hub `/hubs/route-simulation`.

## Out of product scope
- Company, Vehicle, ShipmentRecord and `RouteSimulationService` (fleet module) are kept but out of scope — do not extend or refactor unless asked.
