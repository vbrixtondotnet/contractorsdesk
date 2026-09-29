---
name: page-terminology
description: >-
  Maps ContractorsDesk page names to routes and parses enhancement or bug-fix
  task prompts. Use when the user refers to project details, proposals, Action
  Items Tab, Proposal Tab, Estimate to Actual, or Schedule, or writes a task
  headed ** Enhancements ** or **Bug Fix** with a Project Id and Description.
---

# Page terminology

Use these terms exactly when the user names a page or tab:

project details = /project/{id}
proposals = /proposals/{id}

Action Items Tab = The Action Items Tab under the /project/{id}
Proposal Tab = The Proposal Tab under the /project/{id}
Estimate to Actual = The Estimate to Actual Tab under the /project/{id}
Schedule = The Schedule Tab under the /project/{id}

`proposals` is the proposal editor at `/proposals/{id}`. The Proposal Tab is a tab on project details. Schedule is the Schedule tab on project details (`/project/{id}?t=schedule`). The separate manage-schedule page is `/project/{id}/schedule`.

## Task prompt

The user writes tasks in this format:

** Enhancements **  or **Bug Fix**

Project Id: {id}. This should check the page /project/{id}

Description: The description of the task that you are going to do. This could be an enhancement or a bug fix.

When a message matches this format:

1. Read the heading. ** Enhancements ** is an enhancement. **Bug Fix** is a bug fix.
2. Read Project Id. Inspect `/project/{id}` before changing code. Substitute the given id.
3. Read Description and do that work.
4. When the description names a page or tab, resolve it with the terms above, then open the matching route and view files.

## Where to look

| Term | Route | View |
| --- | --- | --- |
| project details | `/project/{id}` | `Web/ContractorsDesk.Web/Views/Projects/project/` |
| proposals | `/proposals/{id}` | `Web/ContractorsDesk.Web/Views/Proposals/proposal/` |
| Action Items Tab | `/project/{id}?t=action-items` | `Views/Projects/project/components/tab-action-items.cshtml` |
| Proposal Tab | `/project/{id}?t=proposal` | `Views/Projects/project/components/proposal/` |
| Estimate to Actual | `/project/{id}?t=estimate-to-actual` | `Views/Projects/project/components/budget-to-actual/` |
| Schedule | `/project/{id}?t=schedule` | `Views/Projects/project/components/schedule/` |

Tab query values are read by `ProjectsController.Project` as `t`. Tab ids in `project.view.js` are `tab-action-items`, `tab-proposal`, `tab-budget-to-actual`, and `tab-schedule`. Paths under `Views/Projects/project/` are relative to `Web/ContractorsDesk.Web/`.
