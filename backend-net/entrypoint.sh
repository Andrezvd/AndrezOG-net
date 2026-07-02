#!/bin/bash
set -e

# ─────────────────────────────────────────────────────────────────
# AndrezOG — Entrypoint del contenedor backend
# Asegura que el directorio de uploads tenga permisos para el
# usuario no-root 'app' (UID 1654) con el que corre dotnet.
# Luego delega en dotnet backend-net.dll.
# ─────────────────────────────────────────────────────────────────

mkdir -p /app/wwwroot/uploads
chown -R 1654:1654 /app/wwwroot/uploads

exec dotnet backend-net.dll "$@"