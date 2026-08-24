using Imparsable.Lang.Calculator.LSP;
using Microsoft.AspNetCore.Mvc;

namespace Imparsable.API.Controllers;

public class LspController : ControllerBase
{
    [Route("/lsp/clc")]
    public async Task Clc([FromServices] CalculatorLanguageServer server) => await server.ConnectAsync();
}