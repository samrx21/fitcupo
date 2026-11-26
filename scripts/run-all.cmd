@echo off
setlocal EnableExtensions
rem Inicia FitCupo completo, cada componente en su propia ventana:
rem los cuatro microservicios, el API Gateway y la interfaz web.
rem Uso, desde la raiz del repositorio: scripts\run-all.cmd

set "ROOT=%~dp0.."

start "FitCupo - Identity (5170)" cmd /k dotnet run --project "%ROOT%\services\identity\src\Presenters\FitCupo.Identity.API" --launch-profile http
start "FitCupo - Clientes (5180)" cmd /k dotnet run --project "%ROOT%\services\clients\src\Presenters\FitCupo.Clients.API" --launch-profile http
start "FitCupo - Clases (5190)" cmd /k dotnet run --project "%ROOT%\services\classes\src\Presenters\FitCupo.Classes.API" --launch-profile http
start "FitCupo - Reservas (5200)" cmd /k dotnet run --project "%ROOT%\services\bookings\src\Presenters\FitCupo.Bookings.API" --launch-profile http
start "FitCupo - API Gateway (5100)" cmd /k dotnet run --project "%ROOT%\gateway\FitCupo.Gateway" --launch-profile http
start "FitCupo - Web (5300)" cmd /k dotnet run --project "%ROOT%\web\FitCupo.Web" --launch-profile http

echo.
echo Se abrieron seis ventanas. Cuando todas muestren "Now listening on", abre:
echo   Interfaz web:  http://localhost:5300
echo   API Gateway:   http://localhost:5100/api/status
echo   Identity:      http://localhost:5170/scalar
echo   Clientes:      http://localhost:5180/scalar
echo   Clases:        http://localhost:5190/scalar
echo   Reservas:      http://localhost:5200/scalar
endlocal
