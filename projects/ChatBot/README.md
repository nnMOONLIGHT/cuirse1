# CuirseBot — Telegram-бот на ASP.NET Core

Telegram-бот с webhook, интеграцией OpenRouter и хранением истории диалога в памяти.

**Бот:** [@Cuirsebot](https://t.me/Cuirsebot)

## Что реализовано

### Обязательная функциональность
- **Webhook** — контроллер `POST /api/update`, принимает Update от Telegram
- **OpenRouter** — `HttpChatApiClient` через `HttpClientFactory`, настройки через `IOptions<ChatApiSettings>`
- **Команды:** `/start`, `/help`, `/stats`, `/clear`, `/summarize`, `/undo`
- **История** — хранится в `InMemoryChatRepository` по `chatId`, передаётся в LLM при каждом сообщении

### Дополнительная функциональность
- **`/system <текст>`** — добавление системного сообщения (role=system) в историю чата

## Как запустить бота? (Я запускала через Power Shell)

### 1. Была настройка ключей (ИЛИ подробная инструкция как его запустить)

Скопируйте пример конфигурации:


 

- `Telegram:BotToken` — токен от @BotFather (8781642227:AAELlMriNHN5xno_GexlnSLWiuypRVC36Pc)
- `ChatApi:ApiKey` — ключ с [ключ](https://openrouter.ai/keys) 

### 2. Как запускаеться и работает бот терменале povershell?

```bash
cd ChatBot
dotnet run
```

Приложение слушает `http://localhost:5080`.

### 3. Local tunnel

В отдельном терминале:

```bash
npx localtunnel --port 5080
```

Или через ngrok:

```bash
ngrok http 5080
```

### 4. Установка webhook

```bash
curl "https://api.telegram.org/bot<8781642227:AAELlMriNHN5xno_GexlnSLWiuypRVC36Pc>/setWebhook?url=https://<tunnel-url>/api/update"
```

Проверка:

```bash
curl "https://api.telegram.org/bot<8781642227:AAELlMriNHN5xno_GexlnSLWiuypRVC36Pc>/getWebhookInfo"
```

## Структура проекта

```
ChatBot/
├── Controllers/UpdateController.cs   — webhook endpoint
├── Services/
│   ├── HttpChatApiClient.cs          — клиент OpenRouter
│   ├── TelegramClient.cs             — отправка сообщений в Telegram
│   ├── InMemoryChatRepository.cs     — хранение истории
│   └── UpdateHandler.cs              — обработка команд и сообщений
├── Models/                           — настройки и DTO
└── Program.cs                        — DI и конфигурация
```

## Команды бота

| Команда | Описание |
|---------|----------|
| `/start` | Приветствие и инструкция |
| `/help` | Список команд |
| `/stats` | Статистика: сообщения, токены, модель |
| `/clear` | Очистить историю чата |
| `/summarize` | Краткий пересказ диалога |
| `/undo` | Удалить последнюю пару сообщений |
| `/system <текст>` | Задать системный промпт для стиля ответов |

## Автор
Гутман Анастасия
Учебный проект 
