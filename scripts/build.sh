#!/usr/bin/env bash

#The defaults will make the build script testable outside of a GitHub workflow.
GITHUB_OUTPUT="${GITHUB_OUTPUT:-/dev/stdout}"
GITHUB_REPOSITORY_OWNER=${GITHUB_REPOSITORY_OWNER:-mpmxyz}

SCRIPTS_PATH="$(realpath "$( dirname "$0" )")"
BUILD_PATH="$(realpath "$( dirname "$0" )/..")"
#Note: This directory is supposed to be subdirectory within the repository. Do not point ResonitePath to the real Resonite files as they would be deleted in the next step!
export ResonitePath="$BUILD_PATH/ResoniteAssemblies/"

# setup Resonite's reference assemblies
rm -rf "$ResonitePath" 
mkdir -p "$ResonitePath" && cd "$ResonitePath" || exit 1
if ! wget "https://raw.githubusercontent.com/$GITHUB_REPOSITORY_OWNER/ResoniteAssemblies/refs/heads/main/Assemblies.zip"
then
	echo "Make sure you own a fork of https://github.com/mpmxyz/ResoniteAssemblies!" >&2
	echo "While it would be possible to depend on someone else's fork it can be maintained easily without being dependent on other people's actions." >&2
	exit 2
fi
unzip Assemblies.zip || exit 3
cd "$BUILD_PATH" || exit 4
BUILD_NAME="$(basename -- *.slnx .slnx)"

# download packages
dotnet restore || exit 5

# build
dotnet build --no-restore --configuration Release || exit 6

# test
dotnet test --no-restore --no-build || exit 7

# package as zip file
cd "$ResonitePath" || exit 8
zip "$BUILD_NAME.zip" rml_mods/*.dll

RESONITE_VERSION_PATTERN='^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+'
MOD_VERSION_PATTERN='^[0-9]+\.[0-9]+\.[0-9]+'

RESONITE_VERSION="$(cat RESONITE_VERSION | grep -m 1 -Eo "$RESONITE_VERSION_PATTERN" )" || exit $?
MOD_VERSION="$(dotnet script $SCRIPTS_PATH/get-assembly-version.csx rml_mods/*.dll | grep -Eo "$MOD_VERSION_PATTERN" )" || exit $?
echo "resonite_version=$RESONITE_VERSION" >> "$GITHUB_OUTPUT" || exit $?
echo "mod_version=$MOD_VERSION" >> "$GITHUB_OUTPUT" || exit $?
