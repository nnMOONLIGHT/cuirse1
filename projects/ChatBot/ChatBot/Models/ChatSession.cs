namespace ChatBot.Models;

public class ChatSession
{
    public long ChatId { get; set; }
    public string SelectedModel { get; set; } = string.Empty;
    public List<ChatMessage> Messages { get; set; } = new();
    public int TotalTokensUsed { get; set; }
}
