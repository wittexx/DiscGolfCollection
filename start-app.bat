@echo off
echo Starting My Disc Golf Collection App...
echo.

REM Start the API in a new window
echo Starting ASP.NET Core API...
start "API Backend" cmd /k "dotnet run"

REM Wait a moment for the API to start
timeout /t 3 /nobreak >nul

REM Start the Svelte frontend in a new window
echo Starting Svelte Frontend...
start "Svelte Frontend" cmd /k "cd client && npm run dev"

echo.
echo Both servers are starting in separate windows.
echo The web app will be available at: http://localhost:5173
echo.
pause
