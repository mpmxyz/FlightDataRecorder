#!/usr/bin/env bash

RELEASE_TAG_PATTERN="${RELEASE_TAG_PATTERN:-${1:?Expected: \$RELEASE_TAG_PATTERN or \$1}}"

#The defaults will make the build script testable outside of a GitHub workflow.
GITHUB_OUTPUT="${GITHUB_OUTPUT:-/dev/stdout}"
GITHUB_REPOSITORY_OWNER="${GITHUB_REPOSITORY_OWNER:-mpmxyz}"
REPOSITORY_NAME="${REPOSITORY_NAME:-$GITHUB_REPOSITORY_OWNER/FlightDataRecorder}"

echo "Checking if tag matching $RELEASE_TAG_PATTERN exists..."
MATCHES="$(git ls-remote --tags "https://github.com/$REPOSITORY_NAME" "$RELEASE_TAG_PATTERN")"
if [ "$MATCHES" != "" ]
then
	printf "Found:\n%s\n" "$MATCHES"
	if [ "$FAIL_ON_MATCH" = 'true' ]
	then
		echo "::error::Release using the same binary version exists already!" >&2
		exit 1
	else
		echo "::warning::Release using the same binary version exists already!" >&2
		exit 0
	fi
fi
echo "No match!"