<#
Build the Docker image for RealmOfLegends and export it as a tar to the current user's Desktop.

Usage:
  Open PowerShell (as user) and run:
    ./build_and_export_container.ps1

What the script does:
  - Verifies Docker is installed and running
  - Builds the Docker image using the Dockerfile in the repo root and tags it 'realmoflegends:latest'
  - Saves the image as a tar archive to the user's Desktop: realmoflegends.tar

If Docker isn't installed the script prints a message and opens the Docker Desktop download page.
#>

Set-StrictMode -Version Latest

function Abort($message) {
    Write-Host $message -ForegroundColor Red
    exit 1
}

Write-Host "Preparing to build Docker image for RealmOfLegends..."

try {
    $dockerVersion = & docker --version 2>$null
} catch {
    $dockerVersion = $null
}

if (-not $dockerVersion) {
    Write-Host "Docker CLI not found. Please install Docker Desktop for Windows and ensure 'docker' is on PATH." -ForegroundColor Yellow
    Start-Process "https://www.docker.com/get-started"
    Abort "Aborted: Docker required to build the container."
}

Write-Host "Docker found: $dockerVersion"

$desktop = [Environment]::GetFolderPath('Desktop')
$tarPath = Join-Path $desktop 'realmoflegends.tar'

Write-Host "Building Docker image 'realmoflegends:latest'..."
$build = & docker build -t realmoflegends:latest .
if ($LASTEXITCODE -ne 0) { Abort "Docker build failed." }

Write-Host "Saving image to: $tarPath"
& docker save -o $tarPath realmoflegends:latest
if ($LASTEXITCODE -ne 0) { Abort "Failed to save Docker image." }

Write-Host "Image saved successfully. You can load it on another machine with: docker load -i $tarPath" -ForegroundColor Green
