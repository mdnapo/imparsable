using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Protocol.Interfaces;

public interface ICompletionHandler : ILspMethodHandler
{
    public Task HandleAsync(CompletionParams parameters, CancellationToken cancellationToken);
}