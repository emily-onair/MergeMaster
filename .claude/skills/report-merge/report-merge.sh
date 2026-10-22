#!/bin/bash

API_URL="http://localhost:5062/api/add"

URGENT=$1
TAGS="$2"
NOTES="$3"

REPO_NAME=$(basename -s .git "$(git config --get remote.origin.url)")
SOURCE_BRANCH=$(git branch --show-current)
COMMIT_SHA=$(git rev-parse HEAD)
COMMIT_MESSAGE=$(git log -1 --format="%B")
COMMIT_DATE=$(git log -1 --format="%cd" --date=iso-strict)
AUTHOR_NAME=$(git log -1 --format="%an")
AUTHOR_EMAIL=$(git log -1 --format="%ae")

JSON_DATA="{
    \"Repository\": \"$REPO_NAME\",
    \"Branch\": \"$SOURCE_BRANCH\",
    \"CommitSha\": \"$COMMIT_SHA\",
    \"CommitMessage\": \"$COMMIT_MESSAGE\",
    \"commitDate\": \"$COMMIT_DATE\",
    \"AuthorName\": \"$AUTHOR_NAME\",
    \"AuthorEmail\": \"$AUTHOR_EMAIL\",
    \"Urgent\": $URGENT,
    \"Tags\": \"$TAGS\""

if [ -n "$NOTES" ]; then
    JSON_DATA+=", \"Notes\": \"$NOTES\""
fi

JSON_DATA+="}"

HTTP_BODY=$(mktemp)
HTTP_STATUS=$(curl -s -o "$HTTP_BODY" -w "%{http_code}" -X POST "$API_URL" \
    -H "Content-Type: application/json" \
    -d "$JSON_DATA")
CURL_EXIT=$?

if [ $CURL_EXIT -ne 0 ]; then
    echo "RESULT: ERROR"
    echo "REASON: could not reach $API_URL (curl exit $CURL_EXIT). Is the API running?"
    rm -f "$HTTP_BODY"
    exit 1
fi

if [ "$HTTP_STATUS" -ge 200 ] && [ "$HTTP_STATUS" -lt 300 ]; then
    echo "RESULT: OK (HTTP $HTTP_STATUS)"
else
    echo "RESULT: FAILED (HTTP $HTTP_STATUS)"
fi
echo "RESPONSE:"
cat "$HTTP_BODY"
echo
rm -f "$HTTP_BODY"

[ "$HTTP_STATUS" -ge 200 ] && [ "$HTTP_STATUS" -lt 300 ]