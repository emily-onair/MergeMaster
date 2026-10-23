# Fixed responses from GitHub

## Get Repo

https://api.github.com/repos/OWNER/REPO

- repo-ok : 200 OK
- repo-not-found : 404 Not Found

## Get Branch

https://api.github.com/repos/OWNER/REPO/branches/BRANCH

- branch-with-slash-ok : 200 OK (/ symbol will need to be handled in C# logic)
- branch-not-found : 404 Not Found

## Get Commits

https://api.github.com/repos/OWNER/REPO/commits/SHA

- commit-ok : 200 OK
- commit-not-found : 422 Unprocessable Entity (validate SHA before sending request)

## Compare

- compare-in-branch : 200 OK (status => behind/identical)
- compare-not-in-branch : 200 OK (status => ahead/diverged)
- compare-not-found : 404 Not Found (one or both compare objects not found)