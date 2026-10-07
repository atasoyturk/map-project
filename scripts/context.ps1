$ProjectRoot = Split-Path $PSScriptRoot -Parent
Set-Location $ProjectRoot

$OutputDir = Join-Path $ProjectRoot "artifacts"

if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}


Write-Host "================================="
Write-Host "Generating Backend Structure..."
Write-Host "================================="


npx repomix --config repomix/repomix.backend.config.json


Write-Host ""

Write-Host "================================="
Write-Host "Generating Frontend Structure..."
Write-Host "================================="


npx repomix --config repomix/repomix.frontend.config.json


Write-Host ""
Write-Host "================================="
Write-Host "Done."
Write-Host "Files generated in artifacts folder."
Write-Host "================================="