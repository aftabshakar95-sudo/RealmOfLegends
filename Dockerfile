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

# Set environment variables
ENV ASPNETCORE_URLS=http://+:3000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 3000

ENTRYPOINT ["dotnet", "RealmOfLegends.Web.dll"]
