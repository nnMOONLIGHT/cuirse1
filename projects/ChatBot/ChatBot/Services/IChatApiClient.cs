using ChatBot.Models;
using ChatBot.Models.OpenRouter;

namespace ChatBot.Services;

public interface IChatApiClient
{
    Task<ChatCompletionResult> GetCompletionAsync(
        string model,
        IReadOnlyList<ChatMessage> history,
        CancellationToken cancellationToken = default);
}
