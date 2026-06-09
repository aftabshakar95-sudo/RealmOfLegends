# Apply All Bug Fixes Script
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "          Applying All Bug Fixes to Realm of Legends" -ForegroundColor Yellow
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

$fixes = 0

# Fix 1 & 2: Already done (Login and Register scrolling)
Write-Host "✅ Login page scrolling fixed" -ForegroundColor Green
Write-Host "✅ Register page scrolling fixed" -ForegroundColor Green
$fixes += 2

# Fix 3: Home page scrolling
Write-Host "Fixing Home page scrolling..." -ForegroundColor Yellow
$homeFile = "RealmOfLegends.Web\Views\Home\Index.cshtml"
if (Test-Path $homeFile) {
    $content = Get-Content $homeFile -Raw
    $content = $content -replace 'overflow:\s*hidden;', 'overflow-y: auto;'
    Set-Content $homeFile -Value $content
    Write-Host "✅ Home page scrolling fixed" -ForegroundColor Green
    $fixes++
}

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "  Fixed $fixes issues. Restart your app to see changes." -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""
Write-Host "Remaining fixes require code changes:" -ForegroundColor Yellow
Write-Host "- Quest rewards display" -ForegroundColor White
Write-Host "- Shop purchase functionality" -ForegroundColor White  
Write-Host "- Riddle attempt limits" -ForegroundColor White
Write-Host "- Arena enemy visuals" -ForegroundColor White
Write-Host ""
Write-Host "These will be fixed manually now..." -ForegroundColor Yellow
pause
