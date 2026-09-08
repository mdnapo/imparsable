using Imparsable.Lang.Calculator;
using Microsoft.AspNetCore.Mvc;

namespace Imparsable.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CalculatorController : ControllerBase
{
    [HttpGet("grammar", Name = "Grammar")]
    [ProducesResponseType<string>(200)]
    public IActionResult Grammar() => Ok(CalculatorGrammar.Text);
}