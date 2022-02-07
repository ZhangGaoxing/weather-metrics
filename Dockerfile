FROM mcr.microsoft.com/dotnet/core/sdk:6.0-focal-arm32v7 AS build
WORKDIR /app

# publish app
COPY src .
WORKDIR /app/WeatherMetrics.ConsoleApp
RUN dotnet restore
RUN dotnet publish -c release -r linux-arm -o out

## run app
FROM mcr.microsoft.com/dotnet/core/runtime:6.0-focal-arm32v7 AS runtime
WORKDIR /app
COPY --from=build /app/WeatherMetrics.ConsoleApp/out ./

# install native dependencies
RUN apt update && \
    apt install -y --allow-unauthenticated v4l-utils libc6-dev libgdiplus libx11-dev

ENTRYPOINT ["dotnet", "WeatherMetrics.ConsoleApp.dll"]