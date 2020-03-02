FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["ETL-Metrics/ETL-Metrics.csproj", "ETL-Metrics/"]
RUN dotnet restore "ETL-Metrics/ETL-Metrics.csproj"
COPY . .
WORKDIR "/src/ETL-Metrics"
RUN dotnet build "ETL-Metrics.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ETL-Metrics.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ETL-Metrics.dll"]
