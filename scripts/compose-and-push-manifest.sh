#!/usr/bin/env bash

#optional branch name to switch to (empty to use currently checked-out manifest branch)
BRANCH_NAME=${BRANCH_NAME:-}

cd "$( dirname "$0" )" || exit 1
MANIFEST_REPO_PATH="$(realpath -s ../resonite-mod-manifest)"

if ! cd "$MANIFEST_REPO_PATH"
then
	echo "::error::Expected an accessible fork of resonite-modding-group/resonite-mod-manifest at $MANIFEST_REPO_PATH!" >&2
	exit 100
fi

git pull #for non-CI usage

if [ -n "$BRANCH_NAME" ]
then
	git switch "origin/$BRANCH_NAME" >/dev/null || git switch "$BRANCH_NAME" >/dev/null || git switch -c "$BRANCH_NAME" || exit 101
fi

"../scripts/compose-mod-manifest-for-tag.sh" "$@" || exit "$?"
"../scripts/push-manifest.sh" "$@" || exit "$?"