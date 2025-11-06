# PowerShell script to publish the Volunteer Sign-Up System
# Usage: .\publish.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Publishing Volunteer Sign-Up System" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET SDK is installed
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: .NET SDK is not installed or not in PATH" -ForegroundColor Red
    exit 1
}

Write-Host "Found .NET SDK version: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Clean previous publish
if (Test-Path ".\publish") {
    Write-Host "Cleaning previous publish folder..." -ForegroundColor Yellow
    Remove-Item -Recurse -Force ".\publish"
}

# Publish the application
Write-Host "Publishing application in Release mode..." -ForegroundColor Yellow
dotnet publish -c Release -o ./publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Publishing failed" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Publish completed successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Published files are in: .\publish" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Review appsettings.Production.json and update with your production settings" -ForegroundColor White
Write-Host "2. Copy the contents of the 'publish' folder to your web server" -ForegroundColor White
Write-Host "3. Configure IIS (Windows) or Nginx + systemd (Linux)" -ForegroundColor White
Write-Host "4. Set up your subdomain DNS records" -ForegroundColor White
Write-Host "5. See DEPLOYMENT.md for detailed instructions" -ForegroundColor White
Write-Host ""

