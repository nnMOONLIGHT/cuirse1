using ChatBot.Models;
using ChatBot.Models.Telegram;
using Microsoft.Extensions.Options;

namespace ChatBot.Services;

public class UpdateHandler
{
    private readonly IChatRepository _repository;
    private readonly IChatApiClient _chatApiClient;
    private readonly ITelegramClient _telegramClient;
    private readonly ChatApiSettings _settings;
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(
        IChatRepository repository,
        IChatApiClient chatApiClient,
        ITelegramClient telegramClient,
        IOptions<ChatApiSettings> settings,
        ILogger<UpdateHandler> logger)
    {
        _repository = repository;
        _chatApiClient = chatApiClient;
        _telegramClient = telegramClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task HandleAsync(TelegramUpdate update, CancellationToken cancellationToken)
    {
        var message = update.Message;
        if (message?.Text is null)
        {
            return;
        }

        var chatId = message.Chat.Id;
        var text = message.Text.Trim();

        _repository.GetOrCreate(chatId, _settings.DefaultModel);

        try
        {
            if (text.StartsWith('/'))
            {
                await HandleCommandAsync(chatId, text, cancellationToken);
            }
            else
            {
                await HandleUserMessageAsync(chatId, text, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке сообщения из чата {ChatId}", chatId);
            await _telegramClient.SendMessageAsync(
                chatId,
                "Произошла ошибка при обработке запроса. Попробуйте ещё раз позже.",
                cancellationToken);
        }
    }

    private async Task HandleCommandAsync(long chatId, string text, CancellationToken cancellationToken)
    {
        var parts = text.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0].Split('@')[0].ToLowerInvariant();

        switch (command)
        {
            case "/start":
                await _telegramClient.SendMessageAsync(chatId, GetStartMessage(), cancellationToken);
                break;

            case "/help":
                await _telegramClient.SendMessageAsync(chatId, GetHelpMessage(), cancellationToken);
                break;

            case "/stats":
                await HandleStatsAsync(chatId, cancellationToken);
                break;

            case "/clear":
                _repository.ClearHistory(chatId);
                await _telegramClient.SendMessageAsync(chatId, "История сообщений очищена.", cancellationToken);
                break;

            case "/summarize":
                await HandleSummarizeAsync(chatId, cancellationToken);
                break;

            case "/undo":
                await HandleUndoAsync(chatId, cancellationToken);
                break;

            case "/system":
                await HandleSystemAsync(chatId, parts, cancellationToken);
                break;

            default:
                await _telegramClient.SendMessageAsync(
                    chatId,
                    "Неизвестная команда. Введите /help, чтобы увидеть список доступных команд.",
                    cancellationToken);
                break;
        }
    }

    private async Task HandleUserMessageAsync(long chatId, string text, CancellationToken cancellationToken)
    {
        _repository.AddMessage(chatId, new ChatMessage
        {
            Role = "user",
            Content = text
        });

        var session = _repository.GetOrCreate(chatId, _settings.DefaultModel);
        var history = _repository.GetHistory(chatId);

        var result = await _chatApiClient.GetCompletionAsync(session.SelectedModel, history, cancellationToken);

        _repository.AddMessage(chatId, new ChatMessage
        {
            Role = "assistant",
            Content = result.Text
        });

        _repository.AddTokens(chatId, result.TokensUsed);

        await _telegramClient.SendMessageAsync(chatId, result.Text, cancellationToken);
    }

    private async Task HandleStatsAsync(long chatId, CancellationToken cancellationToken)
    {
        var stats = _repository.GetStats(chatId);
        var response =
            $"Статистика чата:\n" +
            $"• Сообщений в истории: {stats.MessageCount}\n" +
            $"• Потрачено токенов: {stats.TotalTokensUsed}\n" +
            $"• Модель: {stats.SelectedModel}";

        await _telegramClient.SendMessageAsync(chatId, response, cancellationToken);
    }

    private async Task HandleSummarizeAsync(long chatId, CancellationToken cancellationToken)
    {
        var history = _repository.GetHistory(chatId);
        if (history.Count == 0)
        {
            await _telegramClient.SendMessageAsync(
                chatId,
                "История пуста — нечего пересказывать.",
                cancellationToken);
            return;
        }

        var session = _repository.GetOrCreate(chatId, _settings.DefaultModel);
        var summaryPrompt = new List<ChatMessage>(history)
        {
            new()
            {
                Role = "user",
                Content = "Кратко перескажи наш диалог в 3–5 предложениях на русском языке."
            }
        };

        var result = await _chatApiClient.GetCompletionAsync(session.SelectedModel, summaryPrompt, cancellationToken);
        _repository.AddTokens(chatId, result.TokensUsed);

        await _telegramClient.SendMessageAsync(
            chatId,
            $"Краткий пересказ диалога:\n\n{result.Text}",
            cancellationToken);
    }

    private async Task HandleUndoAsync(long chatId, CancellationToken cancellationToken)
    {
        var removed = _repository.UndoLastPair(chatId);
        var response = removed
            ? "Последняя пара сообщений (вопрос и ответ) удалена из истории."
            : "Нечего удалять — в истории нет пары сообщений пользователя и ассистента.";

        await _telegramClient.SendMessageAsync(chatId, response, cancellationToken);
    }

    private async Task HandleSystemAsync(long chatId, string[] parts, CancellationToken cancellationToken)
    {
        if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
        {
            await _telegramClient.SendMessageAsync(
                chatId,
                "Использование: /system <текст>\n\nПример: /system Отвечай кратко и дружелюбно.",
                cancellationToken);
            return;
        }

        _repository.AddMessage(chatId, new ChatMessage
        {
            Role = "system",
            Content = parts[1].Trim()
        });

        await _telegramClient.SendMessageAsync(
            chatId,
            "Системное сообщение добавлено в историю. Оно будет влиять на стиль следующих ответов.",
            cancellationToken);
    }

    private static string GetStartMessage() =>
        "Привет! Я CuirseBot — бот-ассистент на базе нейросети.\n\n" +
        "Просто напишите мне сообщение, и я отвечу с учётом истории нашего диалога.\n\n" +
        "Доступные команды:\n" +
        "/help — справка\n" +
        "/stats — статистика чата\n" +
        "/clear — очистить историю\n" +
        "/summarize — краткий пересказ диалога\n" +
        "/undo — отменить последний обмен\n" +
        "/system <текст> — задать стиль ответов";

    private static string GetHelpMessage() =>
        "Список команд:\n\n" +
        "/start — приветствие и краткая инструкция\n" +
        "/help — этот список команд\n" +
        "/stats — количество сообщений и потраченных токенов\n" +
        "/clear — очистить историю текущего чата\n" +
        "/summarize — краткий пересказ диалога\n" +
        "/undo — удалить последнюю пару сообщений\n" +
        "/system <текст> — добавить системное сообщение в историю\n\n" +
        "Любой обычный текст (без команды) отправляется в языковую модель, " +
        "ответ приходит с учётом всей истории переписки.";
}
