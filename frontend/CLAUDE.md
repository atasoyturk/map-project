# frontend — rules

## Stack
React 19 + TypeScript + Vite + Tailwind CSS + OpenLayers. Only `@turf/line-offset` is installed (no full Turf). SignalR client for live vehicles.
Build/lint commands: see root CLAUDE.md (build runs `tsc -b`, so type errors fail the build).
Use npm ≥ 11.19 (CI uses Node 24's npm): older npm on Windows drops Linux-only optional entries (`@emnapi/*`) from `package-lock.json`, and CI's `npm ci` then fails.

## Folder structure (feature-based)
- `src/features/<feature>/` with subfolders `api/`, `components/`, `hooks/`, `pages/`, `types.ts` as needed (also `auth/context/`).
  `map` has sub-folders `core`, `poi`, `transit`, `annotation`, `pages`.
- `src/shared/` — only code used by several features (`api/apiFetch.ts` with the `ApiFetch` type, common components, utils).
- Put new code in the feature it belongs to. Move to `shared/` only when a second feature really needs it.
- A feature does not import from another feature's folders; shared code goes in `shared/`. `shared/` never imports
  from `features/`. Exceptions: `useAuth` from `features/auth/context`; inside `map`, sub-folders may import
  `map/core` and `map/pages` may import all of `map`.
  Existing cross-feature imports are known violations (root rule 6); do not add more.

## Rules
- All backend calls go through the `apiFetch` from `createApiFetch` (adds the JWT, logs out on 401). No raw `fetch` to the backend.
  `shared/api/nominatim.ts` uses raw `fetch` because it calls the external Nominatim service, not the backend.
  Known violation: the heatmap `tileLoadFunction` in `map/core/hooks/useWmsHeatmapLayer` (do not copy).
- API calls live in the feature's `api/` service files, not inside components.
- Map logic (layers, interactions, clicks) lives in hooks (`useXxx`), not in large components.
  Clean up OpenLayers layers, interactions and listeners in the hook's cleanup.
- Geometry is sent/received as WKT in EPSG:4326; the map view is EPSG:3857 — transform explicitly.
- Strong typing: no new `any` (some old code has it — don't fix unasked). Shared types go in the feature's `types.ts`.
- Styling with Tailwind classes in new code; follow the look of existing components.
  Modals whose markup you change use `shared/components/ui/` (Modal, Button, Field); no inline style.
- No hand-written z-index; use `Z` from shared/components/ui/zIndex.ts via `style={{ zIndex: Z.x }}`
  (the only allowed inline style, since Tailwind classes can't take constants).
- Frontend permission checks are only for UX; the backend is the real security check.
- Show/hide UI by permission: `const { hasPermission } = useAuth(); hasPermission("poi_create")`. Admin: `hasPermission("admin_access")`.
  Never add role-name checks (`roles.includes("...")`) — the role set will be simplified. For another user's or role's
  admin status (admin panel lists), use the `hasAdminAccess` field the admin API returns. The remaining "Kullanıcı"
  role-name filters are known (D2 in `planning/tasks.json`); don't fix them unasked.
- POI icons come from the category `Icon` field (sub-categories inherit via `buildCategoryIconMap`); never key icons on category IDs.
- Unit tests (Vitest): co-located `*.test.ts` next to the source; no globals/jsdom.
  When moving a file, move its test with it. E2E tests (Playwright) go in `frontend/e2e/`; credentials from `E2E_EMAIL` / `E2E_PASSWORD`; browser once via `npx playwright install chromium`.
  `npm run e2e` needs the backend running (the user starts it); run it last, after backend build/tests.
- Keep components thin: non-trivial logic (formatting, transforms, calculations)
  goes into pure helpers with co-located Vitest tests.
- Map data from growing tables (POIs) is loaded by current extent or on explicit action; never fetch all rows.
  Known violation: `usePoiLoader` fetches all of `tbl_poi_active` via WFS (do not copy).

## Out of product scope
- `features/company` and the fleet parts of transit (vehicles, shipments, live simulation) are kept but out of scope —
  do not extend or refactor unless asked. Transit stops, trip planning and isochrone ARE in scope.
- Fleet UI is hidden with `FLEET_ENABLED` in `src/shared/featureFlags.ts`. Gate any fleet-related UI with it; never delete fleet code.
