namespace ChatBot.Services;

public interface ITelegramClient
{
    Task SendMessageAsync(long chatId, string text, CancellationToken cancellationToken = default);
}
