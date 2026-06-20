using System.Text.Json.Serialization;

namespace ChatBot.Models.OpenRouter;

public class ChatCompletionRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("messages")]
    public List<ChatCompletionMessage> Messages { get; set; } = new();
}

public class ChatCompletionMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class ChatCompletionResponse
{
    [JsonPropertyName("choices")]
    public List<ChatCompletionChoice> Choices { get; set; } = new();

    [JsonPropertyName("usage")]
    public ChatCompletionUsage? Usage { get; set; }
}

public class ChatCompletionChoice
{
    [JsonPropertyName("message")]
    public ChatCompletionMessage Message { get; set; } = new();
}

public class ChatCompletionUsage
{
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}

public class ChatCompletionResult
{
    public string Text { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
}
