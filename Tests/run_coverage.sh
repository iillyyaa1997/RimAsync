#!/usr/bin/env bash
set -euo pipefail

cd /app

# Ensure base directories exist and minimal assets are present
mkdir -p /app/Source /app/About /app/Planning /app/.cursor/commands || true
# Create tiny placeholder Preview.png if missing
if [ ! -f /app/About/Preview.png ]; then
  echo iVBORw0KGgo= | base64 -d > /app/About/Preview.png || true
fi

# Build tests in Debug so dotnet test places outputs under Debug/net8.0
DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 dotnet build Tests/ --configuration Debug

# Prepare symlinks in test output folder so relative paths used by tests resolve
TARGET_DIR=$(find /app/Tests/bin -type d -path "*/Debug/net8.0" | head -n 1)
mkdir -p "$TARGET_DIR"
for d in Source About Planning .cursor Tests; do
  [ -e "$TARGET_DIR/$d" ] || ln -s "/app/$d" "$TARGET_DIR/$d"
done
[ -e "$TARGET_DIR/README.md" ] || ln -s /app/README.md "$TARGET_DIR/README.md"
[ -e "$TARGET_DIR/CURSOR_COMMANDS.md" ] || ln -s /app/CURSOR_COMMANDS.md "$TARGET_DIR/CURSOR_COMMANDS.md"

# Run tests with coverage without rebuilding
DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 dotnet test Tests/ \
  --no-build \
  --configuration Debug \
  --collect:"XPlat Code Coverage" \
  --results-directory /app/Tests/TestResults/Coverage/ \
  --logger "console;verbosity=normal"
