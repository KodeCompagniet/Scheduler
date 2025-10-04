#!/bin/bash

# NuGet Package Publishing Script
# Publishes DemiCode Scheduler packages to NuGet.org

set -e

echo "========================================"
echo "DemiCode Scheduler NuGet Publish Script"
echo "========================================"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
OUTPUT_DIR="artifacts/nuget"
NUGET_SOURCE="https://api.nuget.org/v3/index.json"
SYMBOL_SOURCE="https://symbols.nuget.org/upload"

# Check if packages exist
if [ ! -d "$OUTPUT_DIR" ] || [ -z "$(ls -A $OUTPUT_DIR/*.nupkg 2>/dev/null)" ]; then
    echo -e "${RED}Error: No packages found in $OUTPUT_DIR${NC}"
    echo -e "${YELLOW}Please run ./pack-nuget.sh first${NC}"
    exit 1
fi

# Check for API key
if [ -z "$NUGET_API_KEY" ]; then
    echo -e "${RED}Error: NUGET_API_KEY environment variable not set${NC}"
    echo -e "${YELLOW}Please set your NuGet API key:${NC}"
    echo -e "${BLUE}  export NUGET_API_KEY=<your-api-key>${NC}"
    exit 1
fi

# Get version from Directory.Build.props
VERSION=$(grep '<Version>' Directory.Build.props | sed 's/.*<Version>\(.*\)<\/Version>.*/\1/')
echo -e "${GREEN}Publishing version: $VERSION${NC}"

# List packages to be published
echo -e "\n${YELLOW}Packages to publish:${NC}"
for package in "$OUTPUT_DIR"/*.nupkg; do
    if [ -f "$package" ]; then
        echo "  - $(basename $package)"
    fi
done

# Confirm before publishing
echo -e "\n${YELLOW}Are you sure you want to publish these packages to NuGet.org? (y/N)${NC}"
read -r response
if [[ ! "$response" =~ ^[Yy]$ ]]; then
    echo -e "${RED}Publishing cancelled${NC}"
    exit 0
fi

# Push packages
echo -e "\n${YELLOW}Publishing packages to NuGet.org...${NC}"
dotnet nuget push "$OUTPUT_DIR/*.nupkg" \
    --api-key "$NUGET_API_KEY" \
    --source "$NUGET_SOURCE" \
    --symbol-source "$SYMBOL_SOURCE" \
    --skip-duplicate

# Check if successful
if [ $? -eq 0 ]; then
    echo -e "\n${GREEN}✓ Packages published successfully!${NC}"
    echo -e "${YELLOW}Next steps:${NC}"
    echo -e "  1. Create a Git tag for version $VERSION:"
    echo -e "     ${BLUE}git tag -a v$VERSION -m \"Release version $VERSION\"${NC}"
    echo -e "     ${BLUE}git push origin v$VERSION${NC}"
    echo -e "  2. Create a GitHub release with the changelog"
    echo -e "  3. Verify packages on ${BLUE}https://www.nuget.org/profiles/DemiCode${NC}"
else
    echo -e "\n${RED}✗ Publishing failed${NC}"
    echo -e "${YELLOW}If you see transient errors, try running the command again${NC}"
    echo -e "${YELLOW}For 409 conflicts (duplicate version), the package may already be published${NC}"
    exit 1
fi