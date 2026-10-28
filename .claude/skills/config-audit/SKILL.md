---
name: config-audit
description: Audit the agent config (CLAUDE.md files, /next skill, code-reviewer) against the repo and report stale or conflicting lines. Use only when the user runs /config-audit.
---

# Config audit

Read-only until the user picks rows.

## 1. Audit
Files: CLAUDE.md, BackendApi/CLAUDE.md, frontend/CLAUDE.md, .claude/skills/next/SKILL.md,
.claude/agents/code-reviewer.md.

- Find the last audit: `git log -1 --grep="Sync agent config with code" --format=%H`.
  If found, verify only claims that mention paths in `git diff --stat <hash>..HEAD`; otherwise verify all.
- Verify every concrete claim (paths, file/class/function names, routes, commands, flags, known exceptions,
  counts) with Grep/Glob. Open a source file only if a claim cannot be checked otherwise.
- Do not run builds or tests.
- Across the five files, look for contradictions, the same rule stated twice, and lines that will go stale
  without adding value.
- "Low-value" means lists that only describe current usage (who uses what). Rules and warnings about
  security, permissions or data visibility are never low-value, even if they name files.

## 2. Report
Table: file:line | problem | evidence (Grep/Glob result or file:line) | proposed text.
Up to 3 rows, most urgent first; fewer is fine. If nothing qualifies, reply only 'No issues found.'
Never add a row without evidence. Stop and wait.


## 3. Apply (only after the user picks rows)
- Apply only the picked rows, exactly as approved. The user approves each config edit when prompted;
  never add a permission rule to skip that.
- Commit only the touched config files with the subject "Sync agent config with code". Do not push.