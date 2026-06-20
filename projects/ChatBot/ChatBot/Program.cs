using ChatBot.Models;
using ChatBot.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ChatApiSettings>(
    builder.Configuration.GetSection(ChatApiSettings.SectionName));
builder.Services.Configure<TelegramSettings>(
    builder.Configuration.GetSection(TelegramSettings.SectionName));

builder.Services.AddSingleton<IChatRepository, InMemoryChatRepository>();
builder.Services.AddScoped<UpdateHandler>();

builder.Services.AddHttpClient<IChatApiClient, HttpChatApiClient>((sp, client) =>
{
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ChatApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddHttpClient<ITelegramClient, TelegramClient>();

builder.Services.AddControllers();
builder.Services.AddLogging();

var app = builder.Build();

app.MapControllers();

app.Run();
