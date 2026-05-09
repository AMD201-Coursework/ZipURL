# Base stage for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["src/ZipURL.Services.ShorterURL/ZipURL.Services.ShorterURL.csproj", "src/ZipURL.Services.ShorterURL/"]
RUN dotnet restore "src/ZipURL.Services.ShorterURL/ZipURL.Services.ShorterURL.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/ZipURL.Services.ShorterURL"
RUN dotnet build "ZipURL.Services.ShorterURL.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ZipURL.Services.ShorterURL.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Environment variables for Render
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ZipURL.Services.ShorterURL.dll"]
