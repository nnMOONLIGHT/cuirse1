@echo off
chcp 65001 >nul
echo === Запуск CuirseBot ===
echo.

start "CuirseBot API" cmd /k "cd /d %~dp0..\ChatBot && dotnet run"

timeout /t 5 /nobreak >nul

echo Запускаю туннель cloudflared...
echo Скопируйте URL вида https://xxx.trycloudflare.com
echo и выполните set-webhook.bat
echo.

"%~dp0cloudflared.exe" tunnel --url http://localhost:5080
