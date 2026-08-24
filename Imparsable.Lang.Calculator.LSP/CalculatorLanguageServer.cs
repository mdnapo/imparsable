using Imparsable.Tools.LSP;
using Imparsable.Tools.LSP.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Imparsable.Lang.Calculator.LSP;

public class CalculatorLanguageServer(
    JsonRpcProvider rpc,
    [FromKeyedServices(nameof(CalculatorLanguageServer))]
    IEnumerable<ILspMethodHandler> handlers
) : LanguageServer(rpc, handlers);