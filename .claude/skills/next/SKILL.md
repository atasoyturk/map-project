---
name: next
description: Runs the AtaGIS work loop for one task — pick it from planning/tasks.json, plan, implement, verify, review, commit. Use when the user types /next or /next <TASK-ID> (e.g. /next T1).
---

# /next — one task, start to finish

The user is the decision-maker and the browser tester. Everything else is your job.
Talk to the user in **Turkish**, briefly. Code, commits and planning files stay in English (UI text in Turkish).

## 1. Pick the task
- Read `planning/tasks.json`. A `requires` id counts as done only if it is in `done`;
  if it is in neither `tasks` nor `done`, stop and ask.
- If a task id is given, use it; otherwise take the first task in array order with `status: "todo"` whose
  `requires` ids are all in `done`. If a required id is still present, stop and tell the user which task blocks it.
- Read only `planning/tasks/<id>.md`; `planning/backlog.md` only if the task points to it.
- Working tree must be clean and on `main`; create `fix/<short-name>` for bugs, `feat/<short-name>` otherwise.

## 2. Plan (gate 1 — wait for the user)
- For a bug: state the root cause in 1–2 lines. If you cannot reproduce or locate it, ask targeted questions.
- Reply with the files you will change (one line each) and ONLY the user's decisions: migration / schema / seed,
  new package, auth or permission behaviour, product or UX choice, anything against the vision. If none, say so.
- Do not write code before the user's OK.

## 3. Implement
- Before applying a migration or running a script that changes data: stop, ask the user to run `scripts/backup.ps1` and wait for 'Backup OK'.
- Problems outside the task: don't fix; add one line to `planning/backlog.md`, or a new task to `tasks.json` +
  `tasks/<id>.md` if it is clearly separate work — tell the user.

## 4. Verify
- Build, lint and test every side you touched with the commands in the root CLAUDE.md.
- Run the `code-reviewer` agent (root rule 7).
  Re-run it only if you changed code after the first review.

## 5. Report (gate 2 — wait for the user)
In Turkish, max 8 lines + test list:
- What changed and why (2–3 lines); remaining reviewer NOTEs (1 line each).
- `git diff --stat` output and the 1–2 design decisions you made (one line each), so the user can review the diff.
- "Tarayıcıda test et:" 2–4 steps (account, screen, expected result). Remind to re-login if JWT claims changed.
- If the user corrected a project convention during this task, propose a one-line
  CLAUDE.md addition; add it only if the user agrees (it goes into the same commit).
Do not commit until the user confirms the browser test passed.

## 6. Close
- Delete the task's entry from `tasks`, append its id to `done`, and delete `planning/tasks/<id>.md`.
- Commit (see root CLAUDE.md rule 8); 2–3 commits only for clearly separate parts.
- `git checkout main`, merge, delete the branch locally. Do not push; tell the user to push.
- End with one line: the next eligible task's id and title, and remind the user to run /clear before the next /next.

## Stop and ask instead of guessing when
- the task is ambiguous or has several reasonable interpretations,
- the change grows beyond the plan (more files or layers),
- a step-2 decision appears mid-work.