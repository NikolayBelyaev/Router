FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY . .

WORKDIR /src/Router.Executable.Admin
RUN dotnet build Router.Executable.Admin.csproj -c Docker -o /app

FROM build AS publish
RUN dotnet publish Router.Executable.Admin.csproj -c Docker -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "Router.Executable.Admin.dll"]
