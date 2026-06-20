using System.Collections.Concurrent;
using ChatBot.Models;

namespace ChatBot.Services;

public class InMemoryChatRepository : IChatRepository
{
    private readonly ConcurrentDictionary<long, ChatSession> _sessions = new();

    public ChatSession GetOrCreate(long chatId, string defaultModel)
    {
        return _sessions.GetOrAdd(chatId, id => new ChatSession
        {
            ChatId = id,
            SelectedModel = defaultModel
        });
    }

    public void AddMessage(long chatId, ChatMessage message)
    {
        var session = _sessions[chatId];
        session.Messages.Add(message);
    }

    public void ClearHistory(long chatId)
    {
        if (_sessions.TryGetValue(chatId, out var session))
        {
            session.Messages.Clear();
        }
    }

    public bool UndoLastPair(long chatId)
    {
        if (!_sessions.TryGetValue(chatId, out var session) || session.Messages.Count == 0)
        {
            return false;
        }

        // Удаляем последнее сообщение ассистента, если есть
        if (session.Messages.Count > 0 && session.Messages[^1].Role == "assistant")
        {
            session.Messages.RemoveAt(session.Messages.Count - 1);
        }

        // Удаляем последнее сообщение пользователя
        if (session.Messages.Count > 0 && session.Messages[^1].Role == "user")
        {
            session.Messages.RemoveAt(session.Messages.Count - 1);
            return true;
        }

        return false;
    }

    public ChatStats GetStats(long chatId)
    {
        if (!_sessions.TryGetValue(chatId, out var session))
        {
            return new ChatStats();
        }

        return new ChatStats
        {
            MessageCount = session.Messages.Count,
            TotalTokensUsed = session.TotalTokensUsed,
            SelectedModel = session.SelectedModel
        };
    }

    public IReadOnlyList<ChatMessage> GetHistory(long chatId)
    {
        return _sessions.TryGetValue(chatId, out var session)
            ? session.Messages.AsReadOnly()
            : Array.Empty<ChatMessage>();
    }

    public void AddTokens(long chatId, int tokens)
    {
        if (_sessions.TryGetValue(chatId, out var session))
        {
            session.TotalTokensUsed += tokens;
        }
    }
}
