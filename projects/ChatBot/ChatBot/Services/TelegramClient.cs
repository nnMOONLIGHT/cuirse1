using System.Text;
using System.Text.Json;
using ChatBot.Models;
using ChatBot.Models.Telegram;
using Microsoft.Extensions.Options;

namespace ChatBot.Services;

public class TelegramClient : ITelegramClient
{
    private readonly HttpClient _httpClient;
    private readonly TelegramSettings _settings;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public TelegramClient(HttpClient httpClient, IOptions<TelegramSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task SendMessageAsync(long chatId, string text, CancellationToken cancellationToken = default)
    {
        var request = new SendMessageRequest
        {
            ChatId = chatId,
            Text = text
        };

        var json = JsonSerializer.Serialize(request, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"https://api.telegram.org/bot{_settings.BotToken}/sendMessage";
        using var response = await _httpClient.PostAsync(url, content, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
