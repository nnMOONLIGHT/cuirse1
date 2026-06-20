using ChatBot.Models.Telegram;
using ChatBot.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Controllers;

[ApiController]
[Route("api")]
public class UpdateController : ControllerBase
{
    private readonly UpdateHandler _updateHandler;
    private readonly ILogger<UpdateController> _logger;

    public UpdateController(UpdateHandler updateHandler, ILogger<UpdateController> logger)
    {
        _updateHandler = updateHandler;
        _logger = logger;
    }

    [HttpPost("update")]
    public async Task<IActionResult> PostUpdate([FromBody] TelegramUpdate update, CancellationToken cancellationToken)
    {
        if (update.Message is null)
        {
            return Ok();
        }

        _logger.LogInformation("Получен update {UpdateId} из чата {ChatId}", update.UpdateId, update.Message.Chat.Id);

        await _updateHandler.HandleAsync(update, cancellationToken);

        return Ok();
    }
}
