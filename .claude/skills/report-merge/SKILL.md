---
name: send-report
description: Use this skill when the user wants to report their current branch and latest commit as ready to merge.
---
## Repository facts (collected automatically)
- Last commit author: !`git log -1 --format="%ae"`
- Current git user: !`git config user.email || echo "not set"`
- Uncommitted changes: !`git status --porcelain | head -20`
- Commits on this branch: !`git log --oneline origin/main..HEAD 2>/dev/null || echo "could not compare with origin/main"`
- Files changed: !`git diff --stat origin/main...HEAD 2>/dev/null || echo "could not compare with origin/main"`

## Instructions
1. If the last commit author differs from the current git user, inform the user they are attempting to request merging someone else's commit and ask whether to continue. If they match, continue.

2. If "Uncommitted changes" lists any files, tell the user which files are not committed, explain that they will NOT be part of the report, and ask whether to continue anyway. If it is empty, continue.

3. Ask the user if they would consider the request "urgent". Interpret their answer as `true` or `false` and set that boolean as the 1st argument when running the script. 

4. Use the following tags to classify the commits and changed files in "Repository facts". If they don't make the purpose clear, open the relevant changed files to understand them. Choose every tag that applies. Pass the confirmed tags as the 2nd argument when running the script, as one comma-separated string with no spaces, in quotes. E.g. "bugfix,docs".
- bugfix: fixes a bug found by investigation, not urgent.
- hotfix: an urgent fix for a production problem.
- feature: adds a new capability: a node, an integration, a page or a service function.
- extend: adds to an existing feature without removing, renaming or changing defaults. Old callers, data and configurations keep working unchanged.
- security: touches encryption, TLS certificates, license checks, inter-service calls or Service.Control.
- chore: maintenance that changes no behavior: dependency updates, build scripts, renames inside one file.
- docs: Documentation for people: READMEs, docs/, changelog wording.
- other: only use this tag when **no other tags** apply.


5. Once you have the details, pass them as arguments to the companion script `.claude/skills/report-merge/report-merge.sh`.

4. Display the API's response to the user.