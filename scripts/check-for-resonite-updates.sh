#!/usr/bin/env bash

#The defaults will make the build script testable outside of a GitHub workflow.
GITHUB_OUTPUT="${GITHUB_OUTPUT:-/dev/stdout}"
GITHUB_REPOSITORY_OWNER="${GITHUB_REPOSITORY_OWNER:-mpmxyz}"
REPOSITORY_NAME="${REPOSITORY_NAME:-$GITHUB_REPOSITORY_OWNER/FlightDataRecorder}"

echo "Checking for current Resonite version..."
SAFE_VERSION_PATTERN='^[a-zA-Z0-9_\.\-]+$' #SECURITY: prevents interpretation when written into $GITHUB_OUTPUT
if ! RESONITE_VERSION="$( curl -s "https://raw.githubusercontent.com/$GITHUB_REPOSITORY_OWNER/ResoniteAssemblies/refs/heads/main/Assemblies/RESONITE_VERSION" | head -n 1 | tr -d '\r\n ' )"
then
	echo "::error::Make sure you own a fork of https://github.com/mpmxyz/ResoniteAssemblies!" >&2
	echo "::notice::While it would be possible to depend on someone else's fork it can be maintained easily without being dependent on other people's actions." >&2
	exit 1
fi
if [[ ! "$RESONITE_VERSION" =~ $SAFE_VERSION_PATTERN ]]
then
	echo "::error::Invalid Resonite version '$RESONITE_VERSION'!" >&2
	exit 2
fi

echo "Searching for release tags built with Resonite $RESONITE_VERSION..."
MATCHING_RELEASES="$( git ls-remote --tags "https://github.com/$REPOSITORY_NAME" "*-$RESONITE_VERSION" )" || exit 3
if printf %s "$MATCHING_RELEASES" | grep -Fq -- "-$RESONITE_VERSION"
then
	printf "Found:\n%s\n" "$MATCHING_RELEASES"
	if [ "$FORCE_RELEASE" != 'true' ]
	then
		if printf %s "$MATCHING_RELEASES" | grep -Fqv -- "failed-$RESONITE_VERSION"
		then
			echo "Cancelling release workflow..."
			echo "UP_TO_DATE=true" >> "$GITHUB_OUTPUT"
			exit 0 #skip build-release but don't fail
		else
			echo "::error::No release found, only failures..."
			exit 99 #fails the workflow so no build is attempted (new pushes with bug fixes trigger build-release directly)
		fi
	else
		echo "Continuing anyway... (FORCE_RELEASE=true)"
	fi
else
	echo "No matching releases found!"
fi

echo "Continuing with release workflow..."
echo "RESONITE_VERSION=$RESONITE_VERSION" >> "$GITHUB_OUTPUT"
