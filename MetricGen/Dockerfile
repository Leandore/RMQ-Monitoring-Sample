FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["MetricGen/MetricGen.csproj", "MetricGen/"]
RUN dotnet restore "MetricGen/MetricGen.csproj"
COPY . .
WORKDIR "/src/MetricGen"
RUN dotnet build "MetricGen.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MetricGen.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MetricGen.dll"]
