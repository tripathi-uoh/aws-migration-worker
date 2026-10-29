# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY src/AwsMigrationWorker/*.csproj ./src/AwsMigrationWorker/
WORKDIR /source/src/AwsMigrationWorker
RUN dotnet restore

WORKDIR /source
COPY src/AwsMigrationWorker/ ./src/AwsMigrationWorker/
WORKDIR /source/src/AwsMigrationWorker
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AwsMigrationWorker.dll"]
