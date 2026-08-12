using Imparsable.LSP.Server.Calculator;
using Microsoft.AspNetCore.Mvc;

namespace Imparsable.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LspController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok();
    
    [Route("/lsp/clc")]
    public async Task Clc([FromServices] CalculatorLanguageServerConnection connection)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        await connection.RunAsync(webSocket, HttpContext.RequestAborted);
    }
}