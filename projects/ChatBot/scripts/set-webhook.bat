@echo off
chcp 65001 >nul
echo === Настройка webhook для CuirseBot ===
echo.

set /p BOT_TOKEN="Введите токен бота Telegram: "
set /p TUNNEL_URL="Введите URL туннеля (например https://xxx.loca.lt): "

curl -s "https://api.telegram.org/bot%BOT_TOKEN%/setWebhook?url=%TUNNEL_URL%/api/update"
echo.
echo.
curl -s "https://api.telegram.org/bot%BOT_TOKEN%/getWebhookInfo"
echo.
