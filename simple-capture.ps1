# Simple Screenshot Capture for Realm of Legends
# Uses Edge browser with manual navigation and automated screenshots

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$BaseUrl = "http://localhost:5138"
$OutputDir = "Documentation\Screenshots"

# Create output directory
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host "   Realm of Legends - Automated Screenshot Capture" -ForegroundColor Cyan
Write-Host "==================================================================" -ForegroundColor Cyan
Write-Host ""

# Function to capture screenshot
function Capture-Screen {
    param([string]$FileName)
    
    $bounds = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds
    $bitmap = New-Object System.Drawing.Bitmap $bounds.Width, $bounds.Height
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.CopyFromScreen($bounds.Location, [System.Drawing.Point]::Empty, $bounds.Size)
    
    $filePath = Join-Path $OutputDir "$FileName.png"
    $bitmap.Save($filePath, [System.Drawing.Imaging.ImageFormat]::Png)
    
    $graphics.Dispose()
    $bitmap.Dispose()
    
    Write-Host "  ✓ Saved: $filePath" -ForegroundColor Green
}

# Launch Edge browser
Write-Host "Launching Edge browser..." -ForegroundColor Yellow
$edge = Start-Process "msedge.exe" -ArgumentList "$BaseUrl", "--start-maximized" -PassThru
Start-Sleep -Seconds 3

$pages = @(
    @{Num="01"; Name="Home"; Url="/"; Desc="Landing Page"},
    @{Num="02"; Name="Login"; Url="/Account/Login"; Desc="Login Page"},
    @{Num="03"; Name="Register"; Url="/Account/Register"; Desc="Registration Page"}
)

Write-Host "`n=== PHASE 1: Public Pages ===" -ForegroundColor Cyan
Write-Host ""

foreach ($page in $pages) {
    Write-Host "[$($page.Num)] $($page.Name) - $($page.Desc)" -ForegroundColor Yellow
    Write-Host "  URL: $BaseUrl$($page.Url)" -ForegroundColor Gray
    Write-Host "  Opening in browser..." -ForegroundColor Gray
    
    # Open URL
    Start-Process "msedge.exe" "$BaseUrl$($page.Url)"
    Start-Sleep -Seconds 3
    
    Write-Host "  Taking screenshot in 2 seconds..." -ForegroundColor Magenta
    Start-Sleep -Seconds 2
    
    Capture-Screen -FileName "$($page.Num)-$($page.Name)"
    Write-Host ""
}

Write-Host "=== PHASE 2: Registration and Character Creation ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "ACTION REQUIRED:" -ForegroundColor Red
Write-Host "1. In the browser, go to Register page" -ForegroundColor Yellow
Write-Host "2. Register with email: test@example.com, password: Test123!" -ForegroundColor Yellow
Write-Host "3. Create a character (any name, choose Warrior class)" -ForegroundColor Yellow
Write-Host ""
$null = Read-Host "Press ENTER when character is created and you're on Dashboard"

# Capture Character Creation page if still visible
Write-Host "`n[04] Create Character Page" -ForegroundColor Yellow
Start-Process "msedge.exe" "$BaseUrl/Account/CreateCharacter"
Start-Sleep -Seconds 3
Capture-Screen -FileName "04-CreateCharacter"

# Authenticated pages
$authPages = @(
    @{Num="05"; Name="Dashboard"; Url="/Dashboard"; Desc="Main Hub"},
    @{Num="06"; Name="Shop"; Url="/Shop"; Desc="Item Marketplace"},
    @{Num="07"; Name="Inventory"; Url="/Inventory"; Desc="Item Management"},
    @{Num="08"; Name="Arena"; Url="/Arena"; Desc="Combat Hub"},
    @{Num="09"; Name="Quests"; Url="/Quests"; Desc="Quest Log"},
    @{Num="10"; Name="Achievements"; Url="/Achievements"; Desc="Achievement Tracker"},
    @{Num="11"; Name="Profile"; Url="/Profile"; Desc="Player Profile"}
)

Write-Host "`n=== PHASE 3: Game Pages ===" -ForegroundColor Cyan
Write-Host ""

foreach ($page in $authPages) {
    Write-Host "[$($page.Num)] $($page.Name) - $($page.Desc)" -ForegroundColor Yellow
    Write-Host "  URL: $BaseUrl$($page.Url)" -ForegroundColor Gray
    
    Start-Process "msedge.exe" "$BaseUrl$($page.Url)"
    Start-Sleep -Seconds 3
    
    Write-Host "  Taking screenshot..." -ForegroundColor Magenta
    Start-Sleep -Seconds 2
    
    Capture-Screen -FileName "$($page.Num)-$($page.Name)"
    Write-Host ""
}

Write-Host "=== PHASE 4: Arena Combat ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "ACTION REQUIRED:" -ForegroundColor Red
Write-Host "1. Go to Arena page: $BaseUrl/Arena" -ForegroundColor Yellow
Write-Host "2. Click Enter Battle button" -ForegroundColor Yellow
Write-Host "3. Wait for battle screen to load" -ForegroundColor Yellow
Write-Host ""
$null = Read-Host "Press ENTER when battle screen is visible"

Write-Host "`n[12] Arena Battle Screen" -ForegroundColor Yellow
Capture-Screen -FileName "12-ArenaBattle"
Start-Sleep -Seconds 2

Write-Host "`nACTION REQUIRED:" -ForegroundColor Red
Write-Host "Click Attack button 5-10 times to complete the battle" -ForegroundColor Yellow
$null = Read-Host "Press ENTER when battle is complete"

Write-Host "`n[13] Arena Battle Result" -ForegroundColor Yellow
Capture-Screen -FileName "13-ArenaBattleResult"

Write-Host "`n=== BONUS: Shop Purchase Flow ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "ACTION REQUIRED:" -ForegroundColor Red
Write-Host "1. Go to Shop: $BaseUrl/Shop" -ForegroundColor Yellow
Write-Host "2. Buy 2-3 items" -ForegroundColor Yellow
$null = Read-Host "Press ENTER when items are purchased"

Write-Host "`n[14] Shop After Purchase" -ForegroundColor Yellow
Capture-Screen -FileName "14-ShopPurchase"

Write-Host "`nACTION REQUIRED:" -ForegroundColor Red
Write-Host "Go to Inventory: $BaseUrl/Inventory" -ForegroundColor Yellow
$null = Read-Host "Press ENTER when on Inventory page"

Write-Host "`n[15] Inventory With Items" -ForegroundColor Yellow
Capture-Screen -FileName "15-InventoryWithItems"

Write-Host "`n==================================================================" -ForegroundColor Green
Write-Host "   Screenshot Capture Complete!" -ForegroundColor Green
Write-Host "==================================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Location: $((Resolve-Path $OutputDir).Path)" -ForegroundColor Cyan
Write-Host "Total screenshots: 15" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next step: Generate Word documentation" -ForegroundColor Yellow
Write-Host ""
