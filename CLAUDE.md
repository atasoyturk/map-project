# AtaGIS — Web GIS Portal

Full-stack web GIS portal: drawing (point/line/polygon), POI, annotations, transit routes & stops,
trip planning, isochrone and location analysis, RBAC + geographic permissions.

**Product direction:** site-selection / location-analysis platform ("where should a chain open its next branch?").
The fleet module (companies, vehicles, shipments, live simulation) is out of scope: kept, hidden, not extended.
Full vision and priorities: `planning/vision.md`, tasks in `planning/tasks.json` (local, not in git — read the vision only for product, UX or scope decisions). Deployment model: one separate installation per customer (single-tenant). Do not add TenantId or multi-customer logic.

## Repo layout
- `BackendApi/` — ASP.NET Core 8 Web API (details: `BackendApi/CLAUDE.md`)
- `frontend/` — React 19 + TypeScript + Vite + Tailwind + OpenLayers (details: `frontend/CLAUDE.md`)

## Runtime dependencies
- PostgreSQL + PostGIS: local Windows service `PostgreSQL18`, not Docker (connection string in user-secrets, not in appsettings)
- GeoServer in Docker at `http://localhost:8090/geoserver`, workspace `gisportal` (container `geoserver`, no volume — never `docker rm` it)
- `tbl_poi_active` and `tbl_transit_stop_active` are not in the database; they are SQL views defined in GeoServer (`featuretype.xml` in its data_dir). Ask before changing them.
- OSRM: public server `https://router.project-osrm.org` (profile `driving`)
- Nominatim (public OSM geocoder), called directly from the browser via `shared/api/nominatim.ts`
- Overpass API: public server `https://overpass-api.de` (admin OSM POI import only)
- Backend runs on `http://localhost:5130`, frontend on `http://localhost:5173`

## Commands
- Backend: `cd BackendApi && dotnet build --nologo -v q` / `dotnet run`
- Frontend: `cd frontend && npm run dev` / `npm run build -- --logLevel error` / `npm run lint -- --quiet` / `npm test`
- GeoServer: `docker start geoserver` (no compose file in the repo)
- After any change, run the build of the side you changed. Do not finish a task with a failing build.
- Backend tests: `dotnet test BackendApi.Tests --nologo -v q` (needs Docker; if Docker is not
  running, stop and tell the user — do not debug it; see `BackendApi/CLAUDE.md` → Tests).
- If dotnet build/test fails because files are locked by a running BackendApi, stop and ask the
  user to stop the server — do not debug it.
- Files over ~300 lines: use the Grep tool (not shell grep, which ignores .ignore) to find the symbol first, then Read only that range (offset/limit).
- While iterating, run only the affected tests: `dotnet test BackendApi.Tests --nologo -v q --filter FullyQualifiedName~<Class>`
  or `npm test -- <file>`. Run the full suite once before the review.

## How to work (always follow)
1. **Think before coding.** State assumptions. If something is unclear or has several interpretations, ask before writing code.
2. **Simplicity first.** Minimum code that solves the request. No extra features, no speculative abstractions or config.
3. **Surgical changes.** Touch only what the task needs. Match existing style. Do not refactor or reformat unrelated code.
   If you see unrelated problems or dead code, mention them — do not fix them unasked.
4. **Verify.** Define how the change will be checked (build, test, or manual steps in the browser) and report the result.
5. For non-trivial tasks, propose a short plan first and wait for approval.
6. The rules in all three CLAUDE.md files apply to new code; places where existing code breaks them are known inconsistencies — do not fix them unasked.
7. Before committing, run the `code-reviewer` agent on the changes; fix every BLOCKER and in-scope SHOULD FIX.
8. Commit messages: short imperative subject only. No "Co-Authored-By" or other trailers.
9. Backend bug fixes start with a failing test that reproduces the bug; new backend logic ships with tests.
   A failing-first test must fail for the expected reason — read the error before writing the fix.

## Never do without asking
- Delete feature code or whole modules (out-of-scope features are hidden from the UI, not deleted)
- Change the database schema or create/apply EF Core migrations
- Change seed data (roles, permissions, POI categories) in `Data/Seed/`
- Add new NuGet or npm packages
- Change auth, permission or GeoServer proxy logic
- Commit, push, or delete files — except steps of a skill that commits after the user's explicit approval
  (e.g. /next step 6, /config-audit step 3). Never push without asking.

## Language
- User-facing messages (errors, UI text) are in **Turkish**. Code, identifiers and comments in English.