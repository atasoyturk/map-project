---
name: next
description: Runs the AtaGIS work loop for one task — pick it from planning/tasks.json, plan, implement, verify, review, commit. Use when the user types /next or /next <TASK-ID> (e.g. /next T1).
---

# /next — one task, start to finish

The user is the decision-maker and the browser tester. Everything else is your job.
Talk to the user in **Turkish**, briefly. Code, commits and planning files stay in English (UI text in Turkish).

## 1. Pick the task
- Read `planning/tasks.json`. Finished tasks are deleted from it, so a `requires` id that is absent counts as done.
- If a task id is given, use it; otherwise take the first task in array order with `status: "todo"` whose
  `requires` ids are all absent. If a required id is still present, stop and tell the user which task blocks it.
- Read only `planning/tasks/<id>.md`. Read `planning/vision.md` only for product, UX or scope decisions;
  `planning/backlog.md` only if the task points to it.
- Working tree must be clean and on `main`; create `fix/<short-name>` for bugs, `feat/<short-name>` otherwise.

## 2. Plan (gate 1 — wait for the user)
- For a bug: state the root cause in 1–2 lines. If you cannot reproduce or locate it, ask targeted questions.
- Reply with the files you will change (one line each) and ONLY the user's decisions: migration / schema / seed,
  new package, auth or permission behaviour, product or UX choice, anything against the vision. If none, say so.
- Do not write code before the user's OK.

## 3. Implement
- Backend bug fix or new backend logic: write the failing test first (if `BackendApi.Tests` exists), then the fix.
- Before applying a migration: stop and ask the user to take a DB backup (pg_dump).
- Problems outside the task: don't fix; add one line to `planning/backlog.md`, or a new task to `tasks.json` +
  `tasks/<id>.md` if it is clearly separate work — tell the user.

## 4. Verify
- Build every side you touched (`dotnet build`, `npm run build`), `npm run lint` for frontend, `dotnet test` if tests exist.
- Run the `code-reviewer` agent. Fix every BLOCKER and in-scope SHOULD FIX.
  Re-run it only if you changed code after the first review.

## 5. Report (gate 2 — wait for the user)
In Turkish, max 5 lines + test list:
- What changed and why (2–3 lines); remaining reviewer NOTEs (1 line each).
- "Tarayıcıda test et:" 2–4 steps (account, screen, expected result). Remind to re-login if JWT claims changed.
Do not commit until the user confirms the browser test passed.

## 6. Close
- Delete the task's entry from `planning/tasks.json` and delete `planning/tasks/<id>.md`.
- Commit with a short imperative English subject, no trailers (2–3 commits only for clearly separate parts).
- `git checkout main`, merge, delete the branch locally. Do not push; tell the user to push.
- End with one line: the next eligible task's id and title.

## Stop and ask instead of guessing when
- the task is ambiguous or has several reasonable interpretations,
- the change grows beyond the plan (more files or layers),
- a step-2 decision appears mid-work.