FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine3.10 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine3.10 AS build
WORKDIR /src
COPY . .
RUN dotnet restore TomKerkhove.Dapr.APIs.Management/TomKerkhove.Dapr.APIs.Management.csproj
COPY . .
WORKDIR "/src/TomKerkhove.Dapr.APIs.Management"
RUN dotnet build TomKerkhove.Dapr.APIs.Management.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish TomKerkhove.Dapr.APIs.Management.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TomKerkhove.Dapr.APIs.Management.dll"]