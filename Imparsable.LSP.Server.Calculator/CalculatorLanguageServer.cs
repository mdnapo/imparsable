using Imparsable.LSP.Protocol;
using Imparsable.LSP.Protocol.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Imparsable.LSP.Server.Calculator;

public class CalculatorLanguageServer(
    JsonRpcProvider rpc,
    ISourceTextBuffer buffer,
    [FromKeyedServices(nameof(CalculatorLanguageServer))]
    IEnumerable<ILspMethodHandler> handlers
) : LanguageServer(rpc, buffer, handlers);