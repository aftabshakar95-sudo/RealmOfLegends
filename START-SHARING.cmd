@echo off
echo ═══════════════════════════════════════════════════════════════════════
echo                   REALM OF LEGENDS - START SHARING
echo ═══════════════════════════════════════════════════════════════════════
echo.
echo This will open 2 windows:
echo   1. Your application
echo   2. ngrok tunnel (your shareable link)
echo.
echo ═══════════════════════════════════════════════════════════════════════
pause

:: Start app
start "Realm of Legends App" cmd /k "cd /d c:\Users\hp\Desktop\RealmOfLegends\RealmOfLegends.Web && dotnet run"

:: Wait for app to start
timeout /t 8 /nobreak >nul

:: Start ngrok
start "ngrok Tunnel" cmd /k "C:\ngrok\ngrok.exe http 5138"

echo.
echo ═══════════════════════════════════════════════════════════════════════
echo   BOTH WINDOWS OPENED!
echo   Check the ngrok window for your shareable link.
echo ═══════════════════════════════════════════════════════════════════════
echo.
pause
