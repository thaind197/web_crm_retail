@echo off
echo ===================================================
echo   SalesCRM Docker Builder ^& Runner
echo ===================================================
echo.
echo Checking Docker status...
"C:\Program Files\Docker\Docker\resources\bin\docker.exe" info >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Docker daemon is not running!
    echo Please open Docker Desktop application first, wait for it to be ready, then run this script again.
    echo.
    pause
    exit /b 1
)

echo [INFO] Docker daemon is running. Building and starting containers...
"C:\Program Files\Docker\Docker\resources\bin\docker-compose.exe" up --build -d

echo.
echo ===================================================
echo   Successfully started services!
echo   - Frontend: http://localhost:3000
echo   - Backend: http://localhost:5000
echo   - Database: localhost:5432
echo   - Redis: localhost:6379
echo ===================================================
echo.
pause
