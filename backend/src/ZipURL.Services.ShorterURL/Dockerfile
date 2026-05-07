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


# # This stage is used when running from VS in fast mode (Default for Debug configuration)
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# USER $APP_UID
# WORKDIR /app
# EXPOSE 8080
# EXPOSE 8081


# # This stage is used to build the service project
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# ARG BUILD_CONFIGURATION=Release
# WORKDIR /src
# COPY ["src/ZipURL.Services.ShorterURL/ZipURL.Services.ShorterURL.csproj", "src/ZipURL.Services.ShorterURL/"]
# RUN dotnet restore "./src/ZipURL.Services.ShorterURL/ZipURL.Services.ShorterURL.csproj"
# COPY . .
# WORKDIR "/src/src/ZipURL.Services.ShorterURL"
# RUN dotnet build "./ZipURL.Services.ShorterURL.csproj" -c $BUILD_CONFIGURATION -o /app/build

# # This stage is used to publish the service project to be copied to the final stage
# FROM build AS publish
# ARG BUILD_CONFIGURATION=Release
# RUN dotnet publish "./ZipURL.Services.ShorterURL.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# # This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "ZipURL.Services.ShorterURL.dll"]