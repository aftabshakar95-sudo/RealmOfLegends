@echo off
echo ========================================
echo Realm of Legends - Publish Script
echo For SmarterASP.NET Deployment
echo ========================================
echo.

REM Step 1: Clean previous builds
echo [1/5] Cleaning previous builds...
dotnet clean
echo.

REM Step 2: Build in Release mode
echo [2/5] Building in Release mode...
dotnet build -c Release
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)
echo.

REM Step 3: Generate database script
echo [3/5] Generating database setup script...
dotnet ef migrations script --idempotent --configuration Release --output "database-setup.sql" --project .\RealmOfLegends.Data\ --startup-project .\RealmOfLegends.Web\
echo.

REM Step 4: Publish application
echo [4/5] Publishing application...
cd RealmOfLegends.Web
dotnet publish -c Release -o bin\Release\net8.0\publish
cd ..
echo.

REM Step 5: Create deployment package
echo [5/5] Creating deployment package...
cd RealmOfLegends.Web\bin\Release\net8.0\publish
if exist "..\..\..\..\..\deployment-package.zip" del "..\..\..\..\..\deployment-package.zip"
powershell Compress-Archive -Path * -DestinationPath "..\..\..\..\..\deployment-package.zip"
cd ..\..\..\..\..\..
echo.

echo ========================================
echo Publish Complete!
echo ========================================
echo.
echo Files ready for deployment:
echo - Published files: RealmOfLegends.Web\bin\Release\net8.0\publish\
echo - ZIP package: deployment-package.zip
echo - Database script: database-setup.sql
echo.
echo Next Steps:
echo 1. Update connection string in appsettings.Production.json
echo 2. Run database-setup.sql on SmarterASP.NET SQL Server
echo 3. Upload deployment-package.zip to SmarterASP.NET
echo 4. Extract in wwwroot folder
echo.
echo See DEPLOYMENT_GUIDE.md for detailed instructions.
echo.
pause
