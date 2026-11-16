#!/usr/bin/env bash

#expectation: working directory is the root of a fork of resonite-modding-group/resonite-mod-manifest

#tag of the release to be added to the manifest
RELEASE_TAG="${RELEASE_TAG:-${1:?Expected: \$RELEASE_TAG or \$1}}"
#mod manifest file
TARGET_FILE="${TARGET_FILE:-manifest/mpmxyz/FlightDataRecorder/info.json}"
#optional author to override user.name/user.email config for the manifest repository (needed for CI/CD)
AUTHOR="${AUTHOR:-}"
#option to disable the push operation (for CI tests)
DRY_RUN="${DRY_RUN:-false}"

git diff
git add "$TARGET_FILE"
if [ -n "$AUTHOR" ]
then
    git config user.name "$AUTHOR"
    git config user.email "$AUTHOR@users.noreply.github.com"
fi
git config push.autoSetupRemote true
git commit -m "Added entry for version $RELEASE_TAG"
if [ "$DRY_RUN" == 'true' ]
then
    echo "::notice::Dry run: skipping push..."
else
    git push
fi