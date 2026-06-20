using ChatBot.Models;

namespace ChatBot.Services;

public interface IChatRepository
{
    ChatSession GetOrCreate(long chatId, string defaultModel);
    void AddMessage(long chatId, ChatMessage message);
    void ClearHistory(long chatId);
    bool UndoLastPair(long chatId);
    ChatStats GetStats(long chatId);
    IReadOnlyList<ChatMessage> GetHistory(long chatId);
    void AddTokens(long chatId, int tokens);
}
