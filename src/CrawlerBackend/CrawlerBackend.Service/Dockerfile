FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CrawlerBackend.Service/CrawlerBackend.Service.csproj", "CrawlerBackend.Service/"]
RUN dotnet restore "CrawlerBackend.Service/CrawlerBackend.Service.csproj"
COPY . .
WORKDIR "/src/CrawlerBackend.Service"
RUN dotnet build "./CrawlerBackend.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CrawlerBackend.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CrawlerBackend.Service.dll"]
