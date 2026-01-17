# Stage 1 - Build and publish
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["TelemetryIntake.slnx", "./"]
COPY ["TelemetryIntake.API/TelemetryIntake.API.csproj", "TelemetryIntake.API/"]
COPY ["TelemetryIntake.Application/TelemetryIntake.Application.csproj", "TelemetryIntake.Application/"]
COPY ["TelemetryIntake.Domain/TelemetryIntake.Domain.csproj", "TelemetryIntake.Domain/"]
COPY ["TelemetryIntake.Infrastructure/TelemetryIntake.Infrastructure.csproj", "TelemetryIntake.Infrastructure/"]

RUN dotnet restore "TelemetryIntake.API/TelemetryIntake.API.csproj"

COPY . .

WORKDIR "/src/TelemetryIntake.API"
RUN dotnet publish "TelemetryIntake.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false


# Stage 2 - Start api
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base

RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

USER app
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:7287

EXPOSE 7287

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TelemetryIntake.API.dll"]