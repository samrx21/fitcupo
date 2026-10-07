@echo off
setlocal EnableExtensions
rem Atajos para las migraciones de EF Core del microservicio de Clientes.
rem Uso: scripts\ef.cmd add NombreMigracion ^| scripts\ef.cmd update [NombreMigracion] ^| scripts\ef.cmd remove

set "ROOT=%~dp0.."
set "PERSISTENCE=%ROOT%\src\Infrastructure\FitCupo.Clients.Persistence"
set "API=%ROOT%\src\Presenters\FitCupo.Clients.API"
cd /d "%ROOT%"

if "%~1"=="" goto usage
if /i "%~1"=="add" goto add
if /i "%~1"=="update" goto update
if /i "%~1"=="remove" goto remove
echo Comando desconocido: %~1
goto usage

:add
if "%~2"=="" (
    echo Error: falta el nombre de la migracion.
    goto usage
)
dotnet ef migrations add %2 --project "%PERSISTENCE%" --startup-project "%API%" --output-dir Migrations
goto end

:update
dotnet ef database update %2 --project "%PERSISTENCE%" --startup-project "%API%"
goto end

:remove
dotnet ef migrations remove --project "%PERSISTENCE%" --startup-project "%API%"
goto end

:usage
echo.
echo Uso: scripts\ef.cmd ^<comando^> [nombre]
echo.
echo   add ^<Nombre^>      Crea una migracion nueva
echo   update [Nombre]   Aplica todas las migraciones, o hasta la indicada
echo   remove            Elimina la ultima migracion creada
exit /b 1

:end
endlocal
