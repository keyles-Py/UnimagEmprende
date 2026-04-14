# ── Etapa 1: build ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto para restaurar dependencias en capa cacheada
COPY EventManager.Domain/EventManager.Domain.csproj           EventManager.Domain/
COPY EventManager.Application/EventManager.Application.csproj EventManager.Application/
COPY EventManager.Infrastructure/EventManager.Infrastructure.csproj EventManager.Infrastructure/
COPY EventManager.API/EventManager.API.csproj                 EventManager.API/

RUN dotnet restore EventManager.API/EventManager.API.csproj

# Copiar el resto del código fuente y publicar
COPY . .
WORKDIR /src/EventManager.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# ── Etapa 2: runtime ───────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EventManager.API.dll"]
