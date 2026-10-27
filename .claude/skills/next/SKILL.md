---
name: next
description: Runs the AtaGIS work loop for one task — pick it from planning/tasks.json, plan, implement, verify, review, commit. Use when the user types /next or /next <TASK-ID> (e.g. /next T1).
---

# /next — one task, start to finish

The user is the decision-maker and the browser tester. Everything else is your job.
Talk to the user in **Turkish**, briefly. Code, commits and planning files stay in English (UI text in Turkish).

## 1. Pick the task
- Read `planning/tasks.json` (small index). If a task id is given, use it. Otherwise take the first task in
  array order with `status: "todo"` whose `requires` are all `"done"`.
- If the given task has unmet `requires`, stop and tell the user which task blocks it.
- Read only that task's file (`planning/tasks/<id>.md`). Read `planning/vision.md` ONLY if the task involves a
  product, UX or scope decision. Do not read `planning/backlog.md` unless the task points to it.
- Make sure the working tree is clean and you are on `main`, then create a branch:
  `fix/<short-name>` for bugs, `feat/<short-name>` otherwise.

## 2. Plan (gate 1 — wait for the user)
- Read only the files the task names, plus what you strictly need. No repo-wide scans.
- For a bug: find the root cause first and state it in 1–2 lines. If you cannot reproduce or locate it,
  ask the user targeted questions instead of guessing.
- Reply with: the files you will change (one line each) and ONLY the decisions that are truly the user's:
  migration / schema / seed change, new package, auth or permission behaviour change, product or UX choice,
  anything that contradicts the vision. If there are none, say so.
- Wait for the user's OK. Do not write code before it.

## 3. Implement
- Follow all CLAUDE.md rules. Smallest change that completes the task.
- Backend bug fix or new backend logic: if `BackendApi.Tests` exists, write the failing test first, then the fix.
- Before applying any migration: stop and ask the user to take a DB backup (pg_dump), then continue.
- Problems you notice outside the task: do not fix them; add one line to `planning/backlog.md`
  (or a new task to `tasks.json` + `tasks/<id>.md` if it is clearly a separate piece of work — tell the user).

## 4. Verify
- Run the build of every side you touched (`dotnet build`, `npm run build`), `npm run lint` for frontend,
  and `dotnet test` if tests exist. Fix failures caused by your change.
- Run the `code-reviewer` agent. Fix every BLOCKER and every in-scope SHOULD FIX, Re-run it only if you changed code after the   first review.

## 5. Report (gate 2 — wait for the user)
Reply in Turkish, max 5 lines plus a short test list:
- What changed and why (2–3 lines).
- Remaining reviewer NOTEs, if any (1 line each).
- "Tarayıcıda test et:" 2–4 concrete steps (which account, which screen, what to expect).
  Remind the user to log out and in again if JWT claims changed.
Then wait. Do not commit until the user confirms the browser test passed.

## 6. Close
After the user confirms:
- In `planning/tasks.json` change only that task: `status: "done"`, `summary: "<one line>"`.
  Delete `planning/tasks/<id>.md`. (planning/ is git-ignored; git history keeps the code record.)
- Commit the code with a short imperative English subject, no trailers. Split into 2–3 commits only if the change
  has clearly separate logical parts.
- `git checkout main`, merge the branch, delete the branch locally. Do not push; tell the user to push.
- End with one line: the next eligible task's id and title.

## Stop and ask instead of guessing when
- the task is ambiguous or has several reasonable interpretations,
- the change grows beyond the task (more files or layers than planned),
- a decision listed in step 2 appears mid-work.
