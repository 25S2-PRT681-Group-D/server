# Use the official .NET 9 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 7001
EXPOSE 7000

# Use the official .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["AgroScan.API.csproj", "./"]
RUN dotnet restore "AgroScan.API.csproj"

# Copy source code
COPY . .
WORKDIR "/src"
RUN dotnet build "AgroScan.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "AgroScan.API.csproj" -c Release -o /app/publish

# Create final runtime image
FROM base AS final
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

# Copy published application
COPY --from=publish /app/publish .

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:7001/health || exit 1

ENTRYPOINT ["dotnet", "AgroScan.API.dll"]
