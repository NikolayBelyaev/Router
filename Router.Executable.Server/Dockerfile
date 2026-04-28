FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim AS base

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY . .

WORKDIR /src/Router.Executable.Server
RUN dotnet build Router.Executable.Server.csproj -c Docker -o /app

FROM build AS publish
RUN dotnet publish Router.Executable.Server.csproj -c Docker -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "Router.Executable.Server.dll"]
