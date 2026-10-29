---
paths:
  - "frontend/**"
---
# Frontend rules

- Only `@turf/line-offset` is installed (no full Turf). Build runs `tsc -b`, so type errors fail the build.
- Use npm ≥ 11.19 (CI uses Node 24's npm): older npm on Windows drops Linux-only optional entries (`@emnapi/*`) from `package-lock.json`, and CI's `npm ci` then fails.

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
- Strong typing: no new `any` (some old code has it — don't fix unasked). Shared types go in the feature's `types.ts`.
- Frontend permission checks are only for UX; the backend is the real security check.
- Show/hide UI by permission: `const { hasPermission } = useAuth(); hasPermission("poi_create")`. Admin: `hasPermission("admin_access")`.
  Never add role-name checks (`roles.includes("...")`) — the role set will be simplified. For another user's or role's
  admin status (admin panel lists), use the `hasAdminAccess` field the admin API returns. The remaining "Kullanıcı"
  role-name filters are known (D2 in `planning/tasks.json`); don't fix them unasked.
- Unit tests (Vitest): co-located `*.test.ts` next to the source; no globals/jsdom. When moving a file, move its test with it.
- Keep components thin: non-trivial logic (formatting, transforms, calculations)
  goes into pure helpers with co-located Vitest tests.

## Out of product scope
- `features/company` and the fleet parts of transit (vehicles, shipments, live simulation) are kept but out of scope —
  do not extend or refactor unless asked. Transit stops, trip planning and isochrone ARE in scope.
- Fleet UI is hidden with `FLEET_ENABLED` in `src/shared/featureFlags.ts`. Gate any fleet-related UI with it; never delete fleet code.
