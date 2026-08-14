using Imparsable.LSP.Protocol;
using Imparsable.LSP.Protocol.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Server.Calculator;

public class CalculatorCompletionHandler(
    ISourceTextBuffer buffer, 
    JsonRpcProvider rpc
) : ICompletionHandler
{
    public Task CompleteAsync(CompletionParams parameters, CancellationToken cancellationToken)
    {
        // var sour
        return Task.CompletedTask;
    }
}