#!/bin/bash

handle_error() {
    echo "------------------------------------------------------------------------"
    echo "FAILURE - Error on line $1"
    echo "------------------------------------------------------------------------"
    exit 1
}

trap 'handle_error $LINENO' ERR

APP_NAME="Cueordinator.app"
PROJECT_DIRECTORY="../src/Pixsper.Cueordinator"
INFO_PLIST="./Info.plist"
ICON_FILE="../Resources/Cueordinator.icns"

echo
echo "------------------------------------------------------------------------"
echo "Cueordinator macOS App Publish Script"
echo "------------------------------------------------------------------------"
echo 

echo "Fetching git tags and getting version.."

git fetch --tags
GIT_VERSION=$(git describe --always --tags --dirty)
SHORT_VERSION_REGEX="^([0-9]+\.[0-9]+\.[0-9]+).*$" 
if [[ $GIT_VERSION =~ $SHORT_VERSION_REGEX ]]
then
    GIT_SHORT_VERSION="${BASH_REMATCH[1]}"
else
    echo "Failed to parse git version from tag '$GIT_VERSION'"
    handle_error
fi

echo "Building Cueordinator $GIT_SHORT_VERSION ($GIT_VERSION) app bundle..."

dotnet publish "$PROJECT_DIRECTORY/Pixsper.Cueordinator.csproj" --configuration Release -r osx-arm64 -v q --nologo
dotnet publish "$PROJECT_DIRECTORY/Pixsper.Cueordinator.csproj" --configuration Release -r osx-x64 -v q --nologo

if [ -d "$APP_NAME" ]
then
    rm -rf "$APP_NAME"
fi

mkdir "$APP_NAME"

mkdir "$APP_NAME/Contents"
mkdir "$APP_NAME/Contents/MacOS"
mkdir "$APP_NAME/Contents/Resources"

lipo -create -output "$APP_NAME/Contents/MacOS/Cueordinator" "$PROJECT_DIRECTORY/bin/Release/net8.0/osx-arm64/publish/Cueordinator" "$PROJECT_DIRECTORY/bin/Release/net8.0/osx-x64/publish/Cueordinator"
cp "$PROJECT_DIRECTORY/bin/Release/net8.0/osx-arm64/publish/libAvaloniaNative.dylib" "$APP_NAME/Contents/MacOS"
cp "$PROJECT_DIRECTORY/bin/Release/net8.0/osx-arm64/publish/libHarfBuzzSharp.dylib" "$APP_NAME/Contents/MacOS"
cp "$PROJECT_DIRECTORY/bin/Release/net8.0/osx-arm64/publish/libSkiaSharp.dylib" "$APP_NAME/Contents/MacOS"
cp "$INFO_PLIST" "$APP_NAME/Contents/Info.plist"
cp "$ICON_FILE" "$APP_NAME/Contents/Resources/$ICON_FILE"

/usr/libexec/PlistBuddy -c "Set :CFBundleVersion $GIT_VERSION" "$APP_NAME/Contents/Info.plist"
/usr/libexec/PlistBuddy -c "Set :CFBundleShortVersionString $GIT_SHORT_VERSION" "$APP_NAME/Contents/Info.plist"

echo
echo "------------------------------------------------------------------------"
echo "Success!"
echo "------------------------------------------------------------------------"
