@echo off
REM Agrega las IPs de MCR al archivo hosts de Windows
REM para evitar problemas de resolución con Docker

echo Agregando entradas de MCR al archivo hosts...

findstr /C:"mcr.microsoft.com" C:\Windows\System32\drivers\etc\hosts >nul
if errorlevel 1 (
    echo.
    echo Se requieren permisos de administrador.
    echo Ejecuta este archivo como ADMINISTRADOR.
    echo.
    pause
    exit /b
)

echo.
echo Las entradas ya existen. No es necesario agregarlas de nuevo.
echo.
pause