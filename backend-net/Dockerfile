# ─────────────────────────────────────────────────────
# Dockerfile — Backend .NET 10 (AndrezOG API)
# Multi-stage build para imagen optimizada
# ─────────────────────────────────────────────────────

# === ETAPA 1: BUILD ===
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend-net.csproj ./
RUN dotnet restore ./backend-net.csproj

COPY . .
RUN dotnet publish ./backend-net.csproj -c Release -o /app/publish /p:UseAppHost=false

# === ETAPA 2: RUNTIME ===
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080

# Instalar curl para healthchecks (útil para Docker)
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Copiar el binario publicado
COPY --from=build /app/publish .

# ─── Estructura de uploads con permisos ─────────────────────
# Crea el directorio base con permisos para el usuario 'app'
# (UID 1654). Para volúmenes existentes con permisos incorrectos,
# ejecutar: docker run --rm -v uploads:/data alpine chown -R 1654:1654 /data
RUN mkdir -p /app/wwwroot/uploads && chown -R 1654:1654 /app/wwwroot/uploads

# Puerto que usará la API dentro del contenedor
EXPOSE 8080

ENTRYPOINT ["dotnet", "backend-net.dll"]
