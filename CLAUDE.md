# AtaGIS — Web GIS Portal

## Never do without asking
- Delete feature code or whole modules (out-of-scope features are hidden from the UI, not deleted)
- Change the database schema or create/apply EF Core migrations
- Change seed data (roles, permissions, POI categories) in `Data/Seed/`
- Add new NuGet or npm packages
- Change auth, permission or GeoServer proxy logic
- Before applying a migration or running a script that changes data, ask the user to run `scripts/backup.ps1`
  and wait for 'Backup OK' (never run it yourself).
- Commit, push, or delete files — except steps of a skill that commits after the user's explicit approval
  (e.g. /next step 6, /config-audit step 3).

## Task intake
- Check `planning/tasks.json` and only the directly relevant task/backlog files for overlap; do not audit the codebase.
- If covered, report the existing task ID and stop. Otherwise, use the next unique ID and create/update only the task index and its short MD.
- Define the problem, expected behavior, scope, and up to 3 observable acceptance criteria. Do not prescribe implementation or test internals; leave those to `/next` planning.
- Ask at most one question, only for a blocking product/security decision. Otherwise record the uncertainty for `/next`.
- Respect the requested mode: plan-only means no edits; explicit task creation means task files only, no application code.
- Validate the task entry and file path. Treat external LLM feedback as advisory; revise only for a material gap or contradiction, not speculative detail.

## How to work (always follow)
1. **Think before coding.** State assumptions. If something is unclear or has several interpretations, ask before writing code.
2. **Simplicity first.** Minimum code that solves the request. No extra features, no speculative abstractions or config.
3. **Surgical changes.** Touch only what the task needs. Match existing style. Do not refactor or reformat unrelated code.
   If you see unrelated problems or dead code, mention them — do not fix them unasked.
   Before creating a file, read an existing file in the same folder (matches style and loads that folder's rules).
4. **Verify.** Define how the change will be checked (build, test, or manual steps in the browser) and report the result.
5. For non-trivial tasks, propose a short plan first and wait for approval.
6. The rules in this file and `.claude/rules/` apply to new code; places where existing code breaks them are known inconsistencies — do not fix them unasked.
7. Before committing, run the `code-reviewer` agent on the changes ONCE; fix every valid BLOCKER and in-scope SHOULD FIX. Do not re-run the reviewer in a loop; if you applied the fixes and the build passes, proceed to commit.
8. Commit messages: short imperative subject only. No "Co-Authored-By" or other trailers.
9. Backend bug fixes start with a failing test that reproduces the bug; new backend logic ships with tests. A failing-first test must fail for the expected reason — read the error before writing the fix. If a test cannot be fixed in 2 attempts, STOP and ask the user to avoid infinite loops.

## Product
Site-selection / location-analysis platform ("where should a chain open its next branch?").
The fleet module (companies, vehicles, shipments, live simulation) is out of scope: kept, hidden, not extended.
Vision: `planning/vision.md`, tasks: `planning/tasks.json` (local, not in git — read the vision only for product, UX or scope decisions).
Single-tenant: one separate installation per customer. Do not add TenantId or multi-customer logic.

## Commands
- Backend: `cd BackendApi && dotnet build --nologo -v q` / `dotnet run`
- Frontend: `cd frontend && npm run dev` / `npm run build -- --logLevel error` / `npm run lint -- --quiet` / `npm test`
- GeoServer: `docker start geoserver` (no compose file in the repo)
- After any change, run the build of the side you changed. Do not finish a task with a failing build. However, if you are trapped and cannot fix a build without breaking a "Never do without asking" rule (e.g., needing a new NuGet package or EF Core migration), STOP and ask the user.
- Backend tests: `dotnet test BackendApi.Tests --nologo -v q` (needs Docker; if Docker is not
  running, stop and tell the user — do not debug it; see `.claude/rules/backend-tests.md`).
- E2E: `cd frontend; npm run e2e` — run by the user (needs the running backend and E2E_EMAIL / E2E_PASSWORD).
  At gate 2, ask for it only when the change touches the login → location-report flow (auth, analysis endpoints/
  services, OSRM, map/core UI); run it after backend build/tests.
- If dotnet build/test fails because files are locked by a running BackendApi, stop and ask the
  user to stop the server — do not debug it.
- Files over ~300 lines: use the Grep tool (not shell grep, which ignores .ignore) to find the symbol first, then Read only that range (offset/limit).
- While iterating, run only the affected tests: `dotnet test BackendApi.Tests --nologo -v q --filter FullyQualifiedName~<Class>`
  or `npm test -- <file>`. Run the full suite once before the review.

## Runtime (not discoverable from the code)
- PostgreSQL + PostGIS: local Windows service `PostgreSQL18`, not Docker (connection string in user-secrets).
- GeoServer container `geoserver` has no volume — removing it loses its config.
- `tbl_poi_active` and `tbl_transit_stop_active` are not in the database; they are SQL views defined in GeoServer (`featuretype.xml` in its data_dir). Ask before changing them.

## Language
- User-facing messages (errors, UI text) are in **Turkish**. Code, identifiers and comments in English.
