using Imparsable.LSP.Server.Calculator;
using Microsoft.AspNetCore.Mvc;

namespace Imparsable.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LspController : ControllerBase
{
    [Route("/lsp/clc")]
    public async Task Clc([FromServices] CalculatorLanguageServerConnection connection) => await connection.RunAsync();
}