@echo off
chcp 65001 >nul
echo === Настройка webhook для CuirseBot ===
echo.

set /p BOT_TOKEN="8781642227:AAELlMriNHN5xno_GexlnSLWiuypRVC36Pc"
set /p TUNNEL_URL="http://localhost:5080"

curl -s "https://api.telegram.org/8781642227:AAELlMriNHN5xno_GexlnSLWiuypRVC36Pc/setWebhook?url=http://localhost:5080/api/update"
echo.
echo.
curl -s "https://api.telegram.org/8781642227:AAELlMriNHN5xno_GexlnSLWiuypRVC36Pc/getWebhookInfo"
echo.
