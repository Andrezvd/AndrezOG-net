#!/bin/bash
set -e

uid=1654
gid=1654

if [ "$(id -u)" = "0" ]; then
    mkdir -p /app/wwwroot/uploads
    chown -R "${uid}:${gid}" /app/wwwroot/uploads
fi

exec dotnet backend-net.dll "$@"