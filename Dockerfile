# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and all project files first
COPY RealmOfLegends.slnx ./
COPY RealmOfLegends.Core/RealmOfLegends.Core.csproj ./RealmOfLegends.Core/
COPY RealmOfLegends.Data/RealmOfLegends.Data.csproj ./RealmOfLegends.Data/
COPY RealmOfLegends.Web/RealmOfLegends.Web.csproj ./RealmOfLegends.Web/

# Restore all projects
RUN dotnet restore RealmOfLegends.Web/RealmOfLegends.Web.csproj

# Copy all source files
COPY RealmOfLegends.Core/ ./RealmOfLegends.Core/
COPY RealmOfLegends.Data/ ./RealmOfLegends.Data/
COPY RealmOfLegends.Web/ ./RealmOfLegends.Web/

# Build and publish
WORKDIR /app/RealmOfLegends.Web
RUN dotnet publish -c Release -o /app/out --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Install small DB client utilities (pg_isready) so the entrypoint can check Postgres readiness
RUN apt-get update \
    && apt-get install -y postgresql-client dos2unix \
    && rm -rf /var/lib/apt/lists/*

# Ensure entrypoint has Unix line endings
RUN if [ -f /app/entrypoint.sh ]; then dos2unix /app/entrypoint.sh || true; fi

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

# Copy entrypoint that waits for DB readiness before starting the app
COPY ./entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/bin/bash", "/app/entrypoint.sh"]
