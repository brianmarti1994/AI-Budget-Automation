using API.Contracts;
using Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<ChatResponse>> Post(
            [FromBody] ChatRequest req,
            [FromServices] IAiCommandParser parser,
            [FromServices] AiCommandExecutor executor,
            CancellationToken ct)
        {

            var currentYear = DateTime.UtcNow.Year;

            var cmd = await parser.ParseAsync(req.Prompt, currentYear, ct);

            // If user clicked confirm in UI, we flip RequiresConfirmation=false and execute delete.
            if (req.Confirm && cmd.Intent == AiIntent.DeleteCategory)
                cmd = cmd with { RequiresConfirmation = false };

            var result = await executor.ExecuteAsync(cmd, currentYear, ct);
            return Ok(new ChatResponse(result.Message, result.NeedsConfirmationd));
        }
    }
}
