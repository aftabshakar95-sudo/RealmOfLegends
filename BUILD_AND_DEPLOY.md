# Deployment Guide for RealmOfLegends

## Problem
Your hosting provider (Railpack/Railway) doesn't support multi-project .NET solutions. It only copies the Web project and ignores Core and Data projects.

## Solutions

### Option 1: Use a Different Hosting Provider (RECOMMENDED)
These platforms properly support multi-project .NET solutions:

1. **Azure App Service** (Best for .NET)
   - Native .NET support
   - Easy deployment from GitHub
   - Free tier available
   - Visit: https://azure.microsoft.com/en-us/products/app-service

2. **Fly.io** (Dockerfile-based)
   - Respects Dockerfiles
   - Simple CLI deployment
   - Free tier includes 3 VMs
   - Visit: https://fly.io

3. **Render.com** (Easy Docker deployment)
   - Dockerfile support
   - GitHub auto-deploy
   - Free tier available
   - Visit: https://render.com

### Option 2: Manual FTP Upload
If you must use your current host:

1. Build locally (already done):
   ```
   dotnet publish RealmOfLegends.Web\RealmOfLegends.Web.csproj -c Release -o publish
   ```

2. Upload the entire `publish` folder contents via FTP

3. Configure your host to:
   - Use .NET 8.0 runtime
   - Set startup command: `dotnet RealmOfLegends.Web.dll`
   - Set environment variable: `ASPNETCORE_URLS=http://0.0.0.0:${PORT}`

### Option 3: Merge Projects into Single Project
Restructure the solution to have all code in one project (complex, not recommended)

## Current Status
- ✅ All code is working locally
- ✅ Build succeeds with no errors
- ✅ `publish` folder contains complete deployment
- ❌ Your hosting provider cannot deploy multi-project solutions

## Recommendation
Switch to Azure App Service or Fly.io for hassle-free deployment with proper .NET support.
