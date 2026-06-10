@echo off
echo ═══════════════════════════════════════════════════════════════════════
echo                   REALM OF LEGENDS - START SHARING
echo ═══════════════════════════════════════════════════════════════════════
echo.
echo This will open 2 windows:
echo   1. Your application (port 5138)
echo   2. ngrok tunnel (your shareable link)
echo.
echo ═══════════════════════════════════════════════════════════════════════
echo.

:: Start app
echo Starting application...
start "Realm of Legends App" cmd /k "cd /d c:\Users\hp\Desktop\RealmOfLegends\RealmOfLegends.Web && dotnet run"

:: Wait for app to start
echo Waiting for application to start (15 seconds)...
timeout /t 15 /nobreak >nul

:: Start ngrok
echo Starting ngrok tunnel...
start "ngrok Tunnel" cmd /k "C:\ngrok\ngrok.exe http 5138 --log=stdout"

echo.
echo ═══════════════════════════════════════════════════════════════════════
echo   BOTH WINDOWS OPENED!
echo   
echo   1. Your app is running at: http://localhost:5138
echo   2. Check the ngrok window for your shareable public URL
echo      (Look for the line: Forwarding https://xxxx.ngrok.io)
echo.
echo   Share the ngrok URL with others to let them access your app!
echo ═══════════════════════════════════════════════════════════════════════
echo.
pause
