using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Protocol.Interfaces;

public interface ICompletionHandler : ILspMethodHandler
{
    public Task CompleteAsync(CompletionParams parameters, CancellationToken cancellationToken);
}