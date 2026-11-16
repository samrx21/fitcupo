@echo off
setlocal EnableExtensions
rem Inicia los tres microservicios de FitCupo, cada uno en su propia ventana.
rem Uso, desde la raiz del repositorio: scripts\run-all.cmd
rem Primero inicia Identity, que emite los JWT que validan Clientes y Clases.

set "ROOT=%~dp0.."

start "FitCupo - Identity (5170)" cmd /k dotnet run --project "%ROOT%\services\identity\src\Presenters\FitCupo.Identity.API" --launch-profile http
start "FitCupo - Clientes (5180)" cmd /k dotnet run --project "%ROOT%\services\clients\src\Presenters\FitCupo.Clients.API" --launch-profile http
start "FitCupo - Clases (5190)" cmd /k dotnet run --project "%ROOT%\services\classes\src\Presenters\FitCupo.Classes.API" --launch-profile http

echo.
echo Se abrieron tres ventanas. Cuando cada una muestre "Now listening on", abre:
echo   Identity: http://localhost:5170/scalar
echo   Clientes: http://localhost:5180/scalar
echo   Clases:   http://localhost:5190/scalar
endlocal
