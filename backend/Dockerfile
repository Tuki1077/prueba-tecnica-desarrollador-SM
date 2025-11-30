# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY LosLadrillosAPI/LosLadrillosAPI.csproj LosLadrillosAPI/
RUN dotnet restore LosLadrillosAPI/LosLadrillosAPI.csproj

# Copy everything else and build
COPY LosLadrillosAPI/ LosLadrillosAPI/
WORKDIR /src/LosLadrillosAPI
RUN dotnet build LosLadrillosAPI.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish LosLadrillosAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5000

# Install SQL Server tools for healthcheck
USER root
RUN apt-get update && apt-get install -y curl gnupg2 ca-certificates && \
    curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor -o /usr/share/keyrings/microsoft-archive-keyring.gpg && \
    echo "deb [arch=arm64,amd64 signed-by=/usr/share/keyrings/microsoft-archive-keyring.gpg] https://packages.microsoft.com/debian/12/prod bookworm main" > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y mssql-tools18 unixodbc-dev && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

USER app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LosLadrillosAPI.dll"]
