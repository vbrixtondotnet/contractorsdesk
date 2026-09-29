---
name: commit-message
description: >-
  Generates a meaningful git commit message from uncommitted changes tied to work
  done in the session. Use when the user says "Commit Message" (exact phrase or
  close variant), asks for a commit message from the diff, or wants message
  text before committing—without creating the commit unless they explicitly ask.
---

# Commit Message

Produce **message text only** unless the user also asks to commit. Focus on changes that match tasks performed in the conversation; ignore unrelated dirty files when that distinction is clear.

## Trigger

When the user says **Commit Message** (case-insensitive), run this workflow and return the message in the response. Do not run `git commit` unless they separately request a commit.

## Workflow

1. **Collect git state** (run in parallel when possible):
   - `git status`
   - `git diff` and `git diff --staged`
   - `git log -5 --oneline` (match subject style and tone)

   On PowerShell, chain with `;` not `&&`. If sandbox blocks git, retry with full permissions.

2. **Scope the diff**
   - Prefer **staged** changes if the user is about to commit staged files only.
   - Otherwise use **unstaged + staged** minus obvious noise unless the user wants everything:
     - Build outputs: `bin/`, `obj/`, `*.dll`, `*.pdb`, `*.dacpac`, `.vs/`
     - Local review artifacts (e.g. `BugBot.results.*`) unless the user included them in the task
   - Cross-check with **conversation history**: summarize the *why* of the work (bug fix, enhancement, refactor), not a file list.

3. **Analyze**
   - One coherent theme: if multiple unrelated edits exist, say so and either propose one message for the task-related subset or offer a short subject per logical group.
   - Classify intent: fix, feat/enhancement, refactor, chore, docs, test.
   - Subject: imperative, concise, explains outcome (not "updated files").
   - Body (optional): 1–3 sentences on behavior, edge cases, or data/SQL impact—only when it helps reviewers.

4. **Output format**

   Default (this repo tends toward plain sentences, not Conventional Commits):

   ```text
   <Subject line in sentence case, ~50–72 chars when practical>

   <Optional body: why and what changed for users/system, not every path touched>
   ```

   If recent `git log` subjects use a pattern (e.g. `fix:`, `feat:`), follow that pattern instead.

   Also provide a **one-line subject only** variant when the change is small.

5. **Do not**
   - Commit, push, or stage unless explicitly requested.
   - Invent changes not present in the diff or session.
   - List every modified filename in the subject.

## Quality checks

Before returning the message:

- [ ] Subject stands alone and matches the main user-visible or system outcome
- [ ] Wording matches recent commit style from `git log`
- [ ] Unrelated or generated files are excluded or called out
- [ ] User can paste the block directly into `git commit -m` / `-F`

## Example

**Session:** Fixed duplicate proposal line items under the same category.

**Output:**

```text
Enforce unique proposal line item names within each category.

Prevent duplicate lines under the same parent (case-insensitive name) in SQL cleanup/sync, services, and proposal/ETA UI; dedupe existing rows and block duplicates when mapping or editing.
```

**Short variant:**

```text
Fix duplicate proposal line items per category by item name
```
