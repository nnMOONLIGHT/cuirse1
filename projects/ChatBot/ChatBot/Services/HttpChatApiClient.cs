using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ChatBot.Models;
using ChatBot.Models.OpenRouter;
using Microsoft.Extensions.Options;

namespace ChatBot.Services;

public class HttpChatApiClient : IChatApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ChatApiSettings _settings;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public HttpChatApiClient(HttpClient httpClient, IOptions<ChatApiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<ChatCompletionResult> GetCompletionAsync(
        string model,
        IReadOnlyList<ChatMessage> history,
        CancellationToken cancellationToken = default)
    {
        var request = new ChatCompletionRequest
        {
            Model = model,
            Messages = history.Select(m => new ChatCompletionMessage
            {
                Role = m.Role,
                Content = m.Content
            }).ToList()
        };

        var json = JsonSerializer.Serialize(request, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
        {
            Content = content
        };

        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        httpRequest.Headers.TryAddWithoutValidation("HTTP-Referer", "https://github.com/nnMOONLIGHT/cuirse1");
        httpRequest.Headers.TryAddWithoutValidation("X-Title", _settings.AppName);

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var completion = JsonSerializer.Deserialize<ChatCompletionResponse>(responseJson, JsonOptions)
            ?? throw new InvalidOperationException("Не удалось разобрать ответ OpenRouter.");

        var text = completion.Choices.FirstOrDefault()?.Message.Content
            ?? throw new InvalidOperationException("OpenRouter вернул пустой ответ.");

        return new ChatCompletionResult
        {
            Text = text,
            TokensUsed = completion.Usage?.TotalTokens ?? 0
        };
    }
}
