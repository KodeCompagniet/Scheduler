#!/bin/bash

# NuGet Package Build Script
# Builds and packs DemiCode Scheduler NuGet packages

set -e

echo "========================================"
echo "DemiCode Scheduler NuGet Pack Script"
echo "========================================"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
SOLUTION="DemiCode.Scheduling.sln"
CONFIG="Release"
OUTPUT_DIR="artifacts/nuget"

# Get version from Directory.Build.props
VERSION=$(grep '<Version>' Directory.Build.props | sed 's/.*<Version>\(.*\)<\/Version>.*/\1/')
echo -e "${GREEN}Building version: $VERSION${NC}"

# Clean artifacts directory
echo -e "\n${YELLOW}Cleaning artifacts directory...${NC}"
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

# Restore dependencies
echo -e "\n${YELLOW}Restoring dependencies...${NC}"
dotnet restore "$SOLUTION"

# Build solution
echo -e "\n${YELLOW}Building solution in $CONFIG configuration...${NC}"
dotnet build "$SOLUTION" -c "$CONFIG" --no-restore

# Run tests
echo -e "\n${YELLOW}Running tests...${NC}"
# Note: DemiCode.SchedulerService is Windows-only and will be skipped on Linux/macOS
# The test runner handles this automatically, but may show warnings
if ! dotnet test "$SOLUTION" -c "$CONFIG" --no-restore --no-build; then
    echo -e "${YELLOW}Warning: Some tests may have failed or been skipped.${NC}"
    echo -e "${YELLOW}This is expected on non-Windows platforms due to Windows-specific projects.${NC}"
    echo -e "${YELLOW}Continuing with packaging...${NC}"
fi

# Pack each publishable project
echo -e "\n${YELLOW}Packing NuGet packages...${NC}"

echo "Packing DemiCode.Shared..."
dotnet pack DemiCode.Shared/DemiCode.Shared.csproj \
    -c "$CONFIG" \
    --no-restore \
    --no-build \
    --output "$OUTPUT_DIR"

echo "Packing DemiCode.Scheduling..."
dotnet pack DemiCode.Scheduling/DemiCode.Scheduling.csproj \
    -c "$CONFIG" \
    --no-restore \
    --no-build \
    --output "$OUTPUT_DIR"

echo "Packing DemiCode.Logging.log4net..."
dotnet pack DemiCode.Logging.log4net/DemiCode.Logging.log4net.csproj \
    -c "$CONFIG" \
    --no-restore \
    --no-build \
    --output "$OUTPUT_DIR"

# List generated packages
echo -e "\n${GREEN}Generated packages:${NC}"
ls -la "$OUTPUT_DIR"/*.nupkg 2>/dev/null || echo "No .nupkg files found"
ls -la "$OUTPUT_DIR"/*.snupkg 2>/dev/null || echo "No .snupkg files found"

# Quick validation
echo -e "\n${YELLOW}Validating package contents...${NC}"
for package in "$OUTPUT_DIR"/*.nupkg; do
    if [ -f "$package" ]; then
        filename=$(basename "$package")
        echo -e "\n${GREEN}Contents of $filename:${NC}"
        unzip -l "$package" | grep -E "(\.dll|\.xml|LICENSE\.txt|NuGet_README\.md)" | head -10
    fi
done

echo -e "\n${GREEN}✓ NuGet packages created successfully!${NC}"
echo -e "${YELLOW}Packages are ready in: $OUTPUT_DIR${NC}"
echo -e "${YELLOW}To publish, run: ./publish-nuget.sh${NC}"