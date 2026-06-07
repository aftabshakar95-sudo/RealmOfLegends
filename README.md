# RealmOfLegends - Local Container & Development Guide

This repository contains the RealmOfLegends Razor Pages application targeting .NET 8.
This guide explains how to build and run the app locally and how to produce a desktop Docker image for sharing.

Prerequisites
- Windows 10/11 or macOS/Linux
- .NET 8 SDK (for local builds): https://dotnet.microsoft.com/download
- Docker Desktop (Windows: enable WSL2 or Hyper-V) : https://www.docker.com/get-started
- Git
- (Optional) Visual Studio 2022/2026 or VS Code

Quick run (recommended using Docker Compose)
1. Start Docker Desktop and ensure it's running.
2. Open PowerShell and run:

   cd "C:\Users\hp\Desktop\RealmOfLegends"
   docker compose up --build

3. After the build and startup completes, the app should be available at:
   http://localhost:8080

Notes about the DB
- docker-compose starts a PostgreSQL container and the web app connects to it using the connection string from docker-compose.yml.
- The app attempts to run EF migrations on startup. The container entrypoint waits for Postgres readiness before starting the app.

Build and export a Docker image tar (shareable)
1. Open PowerShell in the repo root and run:

   .\build_and_export_container.ps1

2. The script will build the image tag `realmoflegends:latest` and save it to your Desktop as `realmoflegends.tar`.
3. On other machines load the image:

   docker load -i C:\Users\hp\Desktop\realmoflegends.tar
   docker run -p 8080:8080 realmoflegends:latest

CI / Docker Hub
- A GitHub Actions workflow (.github/workflows/docker-image.yml) is included. To enable image publishing to Docker Hub, set these secrets in your GitHub repository:
  - DOCKERHUB_USERNAME
  - DOCKERHUB_TOKEN (Docker Hub access token)

Project files added to support containers
- Dockerfile - multi-stage build of the app
- docker-compose.yml - Postgres + web service (development compose)
- entrypoint.sh - waits for Postgres readiness before starting the dotnet app
- build_and_export_container.ps1 - builds and exports image to a tar on your Desktop
- .github/workflows/docker-image.yml - CI workflow to build and push Docker image

Troubleshooting
- If containers fail to start, run `docker-compose logs` or `docker compose logs` to view container logs.
- If migrations fail at startup, the app will log a warning; retry or run migrations manually with `dotnet ef database update`.

If you want me to:
- Provide a PostgreSQL data volume example or backup automation
- Switch back to SQL Server in compose
- Add sample audio assets to wwwroot/audio
- Add automated tests or deployment manifests for Kubernetes

Tell me which one to do next and I'll implement it.
