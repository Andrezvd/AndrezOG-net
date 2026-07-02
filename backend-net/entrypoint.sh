#!/bin/bash
set -e

# ─────────────────────────────────────────────────────────────────
# AndrezOG — Entrypoint del contenedor backend
#
# Si el contenedor se ejecuta como root (user: root en compose),
# preparamos los permisos de uploads antes de delegar.
#
# Si ya corre como usuario no-root (UID 1654, el default de las
# imágenes aspnet), simplemente ejecuta dotnet.
# En ese caso, el directorio /app/wwwroot/uploads debe heredar
# los permisos correctos desde la imagen (ver Dockerfile).
# ─────────────────────────────────────────────────────────────────

if [ "$(id -u)" = "0" ]; then
    mkdir -p /app/wwwroot/uploads
    chown -R 1654:1654 /app/wwwroot/uploads
fi

exec dotnet backend-net.dll "$@"