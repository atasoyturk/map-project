# AtaGIS — Web GIS Portal

Full-stack web GIS portal: drawing (point/line/polygon), POI, annotations, transit routes & stops,
real-time vehicle simulation, trip planning, isochrone and location analysis, RBAC + geographic permissions.

**Product direction:** site-selection / location-analysis platform ("where should a chain open its next branch?").
The fleet module (companies, vehicles, shipments, live simulation) is out of scope: kept, hidden, not extended.
Full vision and priorities: `todo_gis.txt` → ÜRÜN VİZYONU (local file, not in git — read it before planning a feature).

## Repo layout
- `BackendApi/` — ASP.NET Core 8 Web API (details: `BackendApi/CLAUDE.md`)
- `frontend/` — React 19 + TypeScript + Vite + Tailwind + OpenLayers (details: `frontend/CLAUDE.md`)

## Runtime dependencies
- PostgreSQL + PostGIS: local Windows service `PostgreSQL18`, not Docker (connection string in user-secrets, not in appsettings)
- GeoServer in Docker at `http://localhost:8090/geoserver`, workspace `gisportal` (container `geoserver`, no volume — never `docker rm` it)
- `tbl_poi_active` and `tbl_transit_stop_active` are not in the database; they are SQL views defined in GeoServer (`featuretype.xml` in its data_dir). Ask before changing them.
- OSRM: public server `https://router.project-osrm.org` (profile `driving`)
- Nominatim (public OSM geocoder), called directly from the browser for admin region search
- Backend runs on `http://localhost:5130`, frontend on `http://localhost:5173`

## Commands
- Backend: `cd BackendApi && dotnet build` / `dotnet run`
- Frontend: `cd frontend && npm run dev` / `npm run build` / `npm run lint`
- GeoServer: `docker start geoserver` (no compose file in the repo)
- Context dump: `scripts/context.ps1` (repomix → `artifacts/`)
- After any change, run the build of the side you changed. Do not finish a task with a failing build.
<!-- TODO (Ata): add test command once tests exist -->

## How to work (always follow)
1. **Think before coding.** State assumptions. If something is unclear or has several interpretations, ask before writing code.
2. **Simplicity first.** Minimum code that solves the request. No extra features, no speculative abstractions or config.
3. **Surgical changes.** Touch only what the task needs. Match existing style. Do not refactor or reformat unrelated code.
   If you see unrelated problems or dead code, mention them — do not fix them unasked.
4. **Verify.** Define how the change will be checked (build, test, or manual steps in the browser) and report the result.
5. For non-trivial tasks, propose a short plan first and wait for approval.
6. The rules in all three CLAUDE.md files apply to new code; places where existing code breaks them are known inconsistencies — do not fix them unasked.

## Never do without asking
- Delete feature code or whole modules (out-of-scope features are hidden from the UI, not deleted)
- Change the database schema or create/apply EF Core migrations
- Change seed data (roles, permissions, POI categories) in `AppDbContext`
- Add new NuGet or npm packages
- Change auth, permission or GeoServer proxy logic
- Commit, push, or delete files

## Language
- User-facing messages (errors, UI text) are in **Turkish**. Code, identifiers and comments in English.