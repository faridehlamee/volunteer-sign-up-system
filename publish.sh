#!/bin/bash
# Bash script to publish the Volunteer Sign-Up System
# Usage: ./publish.sh

echo "========================================"
echo "Publishing Volunteer Sign-Up System"
echo "========================================"
echo ""

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK is not installed or not in PATH"
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo "Found .NET SDK version: $DOTNET_VERSION"
echo ""

# Clean previous publish
if [ -d "./publish" ]; then
    echo "Cleaning previous publish folder..."
    rm -rf ./publish
fi

# Publish the application
echo "Publishing application in Release mode..."
dotnet publish -c Release -o ./publish

if [ $? -ne 0 ]; then
    echo "Error: Publishing failed"
    exit 1
fi

echo ""
echo "========================================"
echo "Publish completed successfully!"
echo "========================================"
echo ""
echo "Published files are in: ./publish"
echo ""
echo "Next steps:"
echo "1. Review appsettings.Production.json and update with your production settings"
echo "2. Copy the contents of the 'publish' folder to your web server"
echo "3. Configure IIS (Windows) or Nginx + systemd (Linux)"
echo "4. Set up your subdomain DNS records"
echo "5. See DEPLOYMENT.md for detailed instructions"
echo ""

