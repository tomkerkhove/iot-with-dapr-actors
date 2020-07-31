FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.4-alpine3.10 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.202-alpine3.10 AS build
WORKDIR /src
COPY ["TomKerkhove.Dapr.APIs.Management.csproj", ""]

COPY . .
WORKDIR "/src/."
RUN dotnet build "TomKerkhove.Dapr.APIs.Management.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TomKerkhove.Dapr.APIs.Management.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TomKerkhove.Dapr.APIs.Management.dll"]
