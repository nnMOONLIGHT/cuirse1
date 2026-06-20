namespace ChatBot.Models;

public class ChatStats
{
    public int MessageCount { get; set; }
    public int TotalTokensUsed { get; set; }
    public string SelectedModel { get; set; } = string.Empty;
}
