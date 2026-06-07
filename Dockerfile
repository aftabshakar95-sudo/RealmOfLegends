# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and all project files
COPY RealmOfLegends.slnx .
COPY RealmOfLegends.Core/RealmOfLegends.Core.csproj RealmOfLegends.Core/
COPY RealmOfLegends.Data/RealmOfLegends.Data.csproj RealmOfLegends.Data/
COPY RealmOfLegends.Web/RealmOfLegends.Web.csproj RealmOfLegends.Web/

# Restore dependencies for all projects
RUN dotnet restore RealmOfLegends.Web/RealmOfLegends.Web.csproj

# Copy all source code
COPY RealmOfLegends.Core/ RealmOfLegends.Core/
COPY RealmOfLegends.Data/ RealmOfLegends.Data/
COPY RealmOfLegends.Web/ RealmOfLegends.Web/

# Build and publish
WORKDIR /src/RealmOfLegends.Web
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:3000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 3000

ENTRYPOINT ["dotnet", "RealmOfLegends.Web.dll"]
