$ProjectRoot = Split-Path $PSScriptRoot -Parent
Set-Location $ProjectRoot

$OutputDir = Join-Path $ProjectRoot "artifacts"

if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}


Write-Host "================================="
Write-Host "Generating Backend Structure..."
Write-Host "================================="

Get-ChildItem BackendApi -Recurse -File | Where-Object { $_.FullName -notmatch "bin|obj|Debug|Release|\.git|\.vs|TestResults|coverage|packages" } | ForEach-Object { $_.FullName.Replace((Get-Location).Path + "\BackendApi\", "") } | Out-File "$OutputDir\backend_structure.txt"

npx repomix --config repomix/repomix.backend.config.json


Write-Host ""

Write-Host "================================="
Write-Host "Generating Frontend Structure..."
Write-Host "================================="

Get-ChildItem frontend -Recurse -File | Where-Object { $_.FullName -notmatch "node_modules|dist|build|\.git|\.vite|\.next|coverage|\.cache|tmp|temp|logs" } | ForEach-Object { $_.FullName.Replace((Get-Location).Path + "\frontend\", "") } | Out-File "$OutputDir\frontend_structure.txt"

npx repomix --config repomix/repomix.frontend.config.json


Write-Host ""
Write-Host "================================="
Write-Host "Done."
Write-Host "Files generated in artifacts folder."
Write-Host "================================="