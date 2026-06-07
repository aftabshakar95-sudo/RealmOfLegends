# Open All Realm of Legends Deliverables
# This script opens all the generated documentation and the application

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  Realm of Legends - Opening All Deliverables" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# 1. Open the application in browser
Write-Host "1. Opening application in browser..." -ForegroundColor Yellow
Start-Process "http://localhost:5138"
Start-Sleep -Seconds 2

# 2. Open Word document
Write-Host "2. Opening Word documentation..." -ForegroundColor Yellow
$docPath = "Documentation\RealmOfLegends_Complete_Documentation.docx"
if (Test-Path $docPath) {
    Start-Process $docPath
    Write-Host "   ✓ Word document opened" -ForegroundColor Green
} else {
    Write-Host "   ✗ Word document not found" -ForegroundColor Red
}
Start-Sleep -Seconds 2

# 3. Open screenshots folder
Write-Host "3. Opening screenshots folder..." -ForegroundColor Yellow
$screenshotPath = "Documentation\Screenshots"
if (Test-Path $screenshotPath) {
    Start-Process $screenshotPath
    Write-Host "   ✓ Screenshots folder opened" -ForegroundColor Green
} else {
    Write-Host "   ✗ Screenshots folder not found" -ForegroundColor Red
}
Start-Sleep -Seconds 2

# 4. Open final summary
Write-Host "4. Opening final summary..." -ForegroundColor Yellow
$summaryPath = "FINAL_DELIVERABLES_SUMMARY.md"
if (Test-Path $summaryPath) {
    Start-Process "notepad.exe" $summaryPath
    Write-Host "   ✓ Summary opened" -ForegroundColor Green
} else {
    Write-Host "   ✗ Summary not found" -ForegroundColor Red
}
Start-Sleep -Seconds 1

Write-Host ""
Write-Host "================================================" -ForegroundColor Green
Write-Host "  All Deliverables Opened!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host ""
Write-Host "You should now see:" -ForegroundColor Cyan
Write-Host "  • Application running in browser (http://localhost:5138)" -ForegroundColor Gray
Write-Host "  • Word document with 30+ pages" -ForegroundColor Gray
Write-Host "  • Screenshots folder with 13 images" -ForegroundColor Gray
Write-Host "  • Final summary document" -ForegroundColor Gray
Write-Host ""
Write-Host "✨ Enjoy exploring Realm of Legends! ⚔️" -ForegroundColor Yellow
Write-Host ""
