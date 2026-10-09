@echo off
setlocal

echo Building TaPi...

REM Check if .NET SDK is installed
dotnet --version >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: .NET SDK not found.
    pause
    exit /b 1
)

echo Cleaning up previous build artifacts...
rd /s /q "bin" >nul 2>&1
rd /s /q "obj" >nul 2>&1

echo Building x64 executable...
dotnet build "TaPi.csproj" -c Release -p:Platform=x64


if %ERRORLEVEL% NEQ 0 (
    echo.
    echo TaPi build failed.
    pause
    exit /b 1
)

echo.
echo TaPi build succeeded.
exit /b 0
