using Imparsable.Toolchain.LSP;
using Imparsable.Toolchain.LSP.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Imparsable.Lang.Calculator.LSP;

public class CalculatorLanguageServer(
    JsonRpcProvider rpc,
    [FromKeyedServices(nameof(CalculatorLanguageServer))]
    IEnumerable<ILspMethodHandler> handlers
) : LanguageServer(rpc, handlers);