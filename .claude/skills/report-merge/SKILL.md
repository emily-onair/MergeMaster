---
name: report-merge
description: Use this skill when the user wants to report their current branch and latest commit as ready to merge.
---
## Repository facts (collected automatically)
- Current branch: !`git branch --show-current`
- Last commit author: !`git log -1 --format="%ae"`
- Current git user: !`git config user.email || echo "not set"`
- Uncommitted changes: !`git status --porcelain | head -20`
- Commits on this branch: !`git log --oneline origin/main..HEAD 2>/dev/null || echo "could not compare with origin/main"`
- Files changed: !`git diff --stat origin/main...HEAD 2>/dev/null || echo "could not compare with origin/main"`

## Instructions
1. If the current branch is the main, tell the user they are on the main and need to checkout to their feature branch before they can run this skill. Do not proceed.

2. If the last commit author differs from the current git user, inform the user they are attempting to request merging someone else's commit and ask whether to continue. If they match, continue.

3. If "Uncommitted changes" lists any files, tell the user which files are not committed, explain that they will NOT be part of the report, and ask whether to continue anyway. If it is empty, continue.

4. Ask the user if they would consider the request "urgent". Interpret their answer as `true` or `false` and set that boolean as the 1st argument when running the script. 

5. Use the following tags to classify the commits and changed files in "Repository facts". If those are empty, classify the last commit using `git show --stat HEAD`. If they don't make the purpose clear, open the relevant changed files to understand them. Choose every tag that applies yourself; do not ask the user to confirm them. Pass the chosen tags as the 2nd argument when running the script, as one comma-separated string with no spaces, in quotes. E.g. "bugfix,docs".
- bugfix: fixes a bug found by investigation, not urgent.
- hotfix: an urgent fix for a production problem.
- feature: adds a new capability: a node, an integration, a page or a service function.
- extend: adds to an existing feature without removing, renaming or changing defaults. Old callers, data and configurations keep working unchanged.
- security: touches encryption, TLS certificates, license checks, inter-service calls or Service.Control.
- chore: maintenance that changes no behavior: dependency updates, build scripts, renames inside one file.
- docs: Documentation for people: READMEs, docs/, changelog wording.
- claude: changes how Claude works in this repo: CLAUDE.md, anything under .claude/ (skills, agents, commands, hooks, settings) or .mcp.json. Use this instead of feature, chore or docs for those files. Add other tags only for changes outside them in the same commit.
- other: only use this tag when **no other tags** apply.


6. Once you have the details, pass them as arguments to the companion script `.claude/skills/report-merge/report-merge.sh`.

7. Display the API's response to the user.