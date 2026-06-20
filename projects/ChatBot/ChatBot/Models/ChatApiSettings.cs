namespace ChatBot.Models;

public class ChatApiSettings
{
    public const string SectionName = "ChatApi";

    public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1/";
    public string ApiKey { get; set; } = string.Empty;
    public string DefaultModel { get; set; } = "openai/gpt-4o-mini";
    public string AppName { get; set; } = "CuirseBot";
}
