# Realm of Legends - Screenshot Capture Script
# This script captures screenshots of all application pages using Edge browser

param(
    [string]$BaseUrl = "http://localhost:5138",
    [string]$OutputDir = "Documentation\Screenshots"
)

Write-Host "=== Realm of Legends Screenshot Capture ===" -ForegroundColor Cyan
Write-Host "Base URL: $BaseUrl" -ForegroundColor Yellow
Write-Host "Output Directory: $OutputDir" -ForegroundColor Yellow
Write-Host ""

# Create output directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$fullOutputPath = Join-Path $scriptPath $OutputDir
if (-not (Test-Path $fullOutputPath)) {
    New-Item -ItemType Directory -Path $fullOutputPath -Force | Out-Null
    Write-Host "Created output directory: $fullOutputPath" -ForegroundColor Green
}

# Function to take screenshot using Edge
function Capture-Screenshot {
    param(
        [string]$Url,
        [string]$FileName,
        [int]$WaitSeconds = 3
    )
    
    $outputFile = Join-Path $fullOutputPath "$FileName.png"
    
    Write-Host "Capturing: $FileName..." -ForegroundColor Yellow
    Write-Host "  URL: $Url" -ForegroundColor Gray
    
    # Open URL in Edge
    Start-Process "msedge.exe" $Url
    
    # Wait for page to load
    Start-Sleep -Seconds $WaitSeconds
    
    Write-Host "  Screenshot would be saved to: $outputFile" -ForegroundColor Gray
    Write-Host "  (Manual screenshot needed - Press Alt+PrtScn when ready)" -ForegroundColor Magenta
    Write-Host "  Press Enter to continue..." -ForegroundColor Cyan
    Read-Host
    
    return $outputFile
}

# Page list
$pages = @(
    @{Name="01-Home"; Url="$BaseUrl/"; Wait=3},
    @{Name="02-Login"; Url="$BaseUrl/Account/Login"; Wait=2},
    @{Name="03-Register"; Url="$BaseUrl/Account/Register"; Wait=2}
)

Write-Host "=== Phase 1: Public Pages ===" -ForegroundColor Cyan
foreach ($page in $pages) {
    Capture-Screenshot -Url $page.Url -FileName $page.Name -WaitSeconds $page.Wait
}

Write-Host ""
Write-Host "=== Registration Instructions ===" -ForegroundColor Cyan
Write-Host "1. Please register a new account in the browser" -ForegroundColor Yellow
Write-Host "2. Create a character (choose any class)" -ForegroundColor Yellow
Write-Host "3. Press Enter when you reach the Dashboard" -ForegroundColor Yellow
Read-Host "Press Enter to continue"

# Authenticated pages
$authPages = @(
    @{Name="04-CreateCharacter"; Url="$BaseUrl/Account/CreateCharacter"; Wait=2},
    @{Name="05-Dashboard"; Url="$BaseUrl/Dashboard"; Wait=3},
    @{Name="06-Shop"; Url="$BaseUrl/Shop"; Wait=3},
    @{Name="07-Inventory"; Url="$BaseUrl/Inventory"; Wait=2},
    @{Name="08-Arena"; Url="$BaseUrl/Arena"; Wait=2},
    @{Name="09-Quests"; Url="$BaseUrl/Quests"; Wait=3},
    @{Name="10-Achievements"; Url="$BaseUrl/Achievements"; Wait=3},
    @{Name="11-Profile"; Url="$BaseUrl/Profile"; Wait=2}
)

Write-Host ""
Write-Host "=== Phase 2: Game Pages ===" -ForegroundColor Cyan
foreach ($page in $authPages) {
    Capture-Screenshot -Url $page.Url -FileName $page.Name -WaitSeconds $page.Wait
}

Write-Host ""
Write-Host "=== Phase 3: Arena Battle ===" -ForegroundColor Cyan
Write-Host "1. Navigate to Arena page" -ForegroundColor Yellow
Write-Host "2. Click 'Enter Battle'" -ForegroundColor Yellow
Write-Host "3. Take screenshot of battle screen" -ForegroundColor Yellow
Write-Host "4. Click 'Attack' multiple times" -ForegroundColor Yellow
Write-Host "5. Take screenshot of victory/defeat screen" -ForegroundColor Yellow
Read-Host "Press Enter when ready"

Capture-Screenshot -Url "$BaseUrl/Arena" -FileName "12-ArenaBattle" -WaitSeconds 2
Capture-Screenshot -Url "$BaseUrl/Arena/Battle" -FileName "13-ArenaCombat" -WaitSeconds 3

Write-Host ""
Write-Host "=== Screenshot Capture Complete! ===" -ForegroundColor Green
Write-Host "Screenshots location: $fullOutputPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Review all screenshots in the output folder" -ForegroundColor Gray
Write-Host "2. Run the documentation generation script" -ForegroundColor Gray
Write-Host ""
