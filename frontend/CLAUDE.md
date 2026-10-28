# frontend — rules

## Stack
React 19 + TypeScript + Vite + Tailwind CSS + OpenLayers. Only `@turf/line-offset` is installed (no full Turf). SignalR client for live vehicles.
Build/lint commands: see root CLAUDE.md (build runs `tsc -b`, so type errors fail the build).

## Folder structure (feature-based)
- `src/features/<feature>/` with subfolders `api/`, `components/`, `hooks/`, `pages/`, `types.ts` as needed (also `auth/context/`, `admin/helpers/`).
  Features: `admin`, `auth`, `company`, `map/{core, poi, transit, annotation}`.
- `src/shared/` — only code used by several features (`api/apiFetch.ts`, `api/signalR.ts`, common components, utils).
- Today `shared/api/` also holds the poi, transit, company, trip-planning and thematic-map services; those features have no `api/` folder.
- `src/utils/mapStyle.ts` — shared OpenLayers style builders; lives outside `shared/`.
- Put new code in the feature it belongs to. Move to `shared/` only when a second feature really needs it.

## Rules
- All backend calls go through the `apiFetch` from `createApiFetch` (adds the JWT, logs out on 401). No raw `fetch` to the backend.
  `features/admin/api/nominatim.ts` uses raw `fetch` because it calls the external Nominatim service, not the backend.
- API calls live in the feature's `api/` service files, not inside components.
- Map logic (layers, interactions, clicks) lives in hooks (`useXxx`), not in large components.
  Clean up OpenLayers layers, interactions and listeners in the hook's cleanup.
- Geometry is sent/received as WKT in EPSG:4326; the map view is EPSG:3857 — transform explicitly.
- Strong typing: no new `any` (some old code has it — don't fix unasked). Shared types go in the feature's `types.ts`.
- Styling with Tailwind classes in new code; follow the look of existing components.
  Some older components use inline `style={{...}}` objects — don't convert them unasked.
- UI text is in Turkish.
- Frontend permission checks are only for UX; the backend is the real security check.
- Show/hide UI by permission: `const { hasPermission } = useAuth(); hasPermission("poi_create")`. Admin: `hasPermission("admin_access")`.
  Never add role-name checks (`roles.includes("...")`) — the role set will be simplified. For another user's or role's
  admin status (admin panel lists), use the `hasAdminAccess` field the admin API returns. The remaining "Kullanıcı"
  role-name filters are known (D2 in `planning/tasks.json`); don't fix them unasked.
- POI icons: do not add IDs to the hardcoded category ID sets in `usePoiLoader.ts`; a per-category icon field is planned (see task B1).
- Unit tests (Vitest): co-located `*.test.ts` next to the source; no globals/jsdom.
  When moving a file, move its test with it. E2E tests (later) go in `frontend/e2e/`.

## Out of product scope
- `features/company` and the fleet parts of transit (vehicles, shipments, live simulation) are kept but out of scope —
  do not extend or refactor unless asked. Transit stops, trip planning and isochrone ARE in scope.
- Fleet UI is hidden with `FLEET_ENABLED` in `src/shared/featureFlags.ts`. Gate any fleet-related UI with it; never delete fleet code.
