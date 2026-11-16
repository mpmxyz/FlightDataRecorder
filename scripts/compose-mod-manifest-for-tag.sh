#!/usr/bin/env bash

#expectation: Working directory is the root of a fork of resonite-modding-group/resonite-mod-manifest.
#expectation: The fork is checked out to a child directory of the mod's repository.

#tag of the release to be added to the manifest
RELEASE_TAG="${RELEASE_TAG:-${1:?Expected: \$RELEASE_TAG or \$1}}"
#current mod description with version template
TEMPLATE_FILE="${TEMPLATE_FILE:-../rml-mod.json}"
#mod manifest file
TARGET_FILE="${TARGET_FILE:-manifest/mpmxyz/FlightDataRecorder/info.json}"
#set to "true" to allow replacing an existing version with updated data ("false" would fail in such situations.)
OVERRIDE_SAME_VERSION="${OVERRIDE_SAME_VERSION:-false}"
#avoids downloading and hashing of files for testing purposes
INJECT_DUMMY_HASHSUMS="${INJECT_DUMMY_HASHSUMS:-false}"

#hardcoded pattern as it is not customizable but a fixed part of the release logic anyway
PATTERN="v([0-9]\\.[0-9]\\.[0-9])-.*"

if [[ "$RELEASE_TAG" =~ $PATTERN ]]
then
	VERSION="${BASH_REMATCH[1]}"
	echo "$VERSION"

	if [ -r "$TARGET_FILE" ]
	then
		PREVIOUS_MANIFEST="$(jq . "$TARGET_FILE")" || exit 1
	else
		PREVIOUS_MANIFEST='{}'
	fi

	if jq -n --exit-status '$manifest.versions | has($version)' --argjson manifest "$PREVIOUS_MANIFEST" --arg version "$VERSION" >/dev/null
	then
		if [ "$OVERRIDE_SAME_VERSION" = 'true' ]
		then
			echo "::warning::Version already exists!"
			echo "::notice::Overriding..."
		else
			echo "::error::Version already exists!" >&2
			exit 2
		fi
	fi

	TEMPLATE="$(jq . "$TEMPLATE_FILE")" || exit 3
	echo "Template: $TEMPLATE"

#	echo "$VERSION_TEMPLATE"
	#Insert Versions
	RAW_VERSION_DESCRIPTION="$(jq -n '$template.versionTemplate | walk(if type == "string" then gsub("%VERSION%"; $version) | gsub("%TAG%"; $tag) else . end)' --arg tag "$RELEASE_TAG" --arg version "$VERSION" --argjson template "$TEMPLATE" )" || exit 4
	echo "Raw version description: $RAW_VERSION_DESCRIPTION"
	
	echo "Extracting URLs..."
	mapfile -d '' URLS < <( jq -n --raw-output0 '$description.artifacts[].url' --argjson description "$RAW_VERSION_DESCRIPTION" ) || exit 5
	
	echo "Computing hashsums..."
	declare -a HASHSUMS
	for URL in "${URLS[@]}"
	do
		echo "Hashing $URL..."
		if [ "$INJECT_DUMMY_HASHSUMS" != 'true' ]
		then
			HASHSUM="$( set -o pipefail ; curl --location --fail "$URL" | sha256sum | grep -m 1 -Eo '^\S+' )" || exit 6
		else
			HASHSUM="dummy"
			echo "::notice::Injected dummy hashsums for testing purposes"
		fi
		echo "->$HASHSUM"
		HASHSUMS+=( "$HASHSUM" )
	done
	
	echo "Injecting Hashsums..."
	VERSION_DESCRIPTION="$(jq -n '$description | .artifacts |= (to_entries | map(.value.sha256=$ARGS.positional[.key] | .value) )' --argjson description "$RAW_VERSION_DESCRIPTION" --args "${HASHSUMS[@]}")" || exit 7
	echo "Final version description: $VERSION_DESCRIPTION"
	
	echo "Merging into existing..."
	MERGED_MANIFEST="$( jq -n '$template | del(.versionTemplate) | .versions=$manifest.versions | .versions[$version] = $versionDescription' --argjson template "$TEMPLATE" --argjson manifest "$PREVIOUS_MANIFEST" --arg version "$VERSION" --argjson versionDescription "$VERSION_DESCRIPTION" )" || exit 8

	diff -bu <( printf %s "$PREVIOUS_MANIFEST" ) <( printf %s "$MERGED_MANIFEST")

	mkdir -p "$(dirname "$TARGET_FILE")" || exit 9
	echo "$MERGED_MANIFEST" > "$TARGET_FILE" || exit 10
else
	echo "::error::Tag $RELEASE_TAG does not match expected pattern $PATTERN!"
	exit 11
fi
