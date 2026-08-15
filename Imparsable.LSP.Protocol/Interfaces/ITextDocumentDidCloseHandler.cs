using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Protocol.Interfaces;

public interface ITextDocumentDidCloseHandler : ILspMethodHandler
{
    public Task HandleAsync(DidCloseTextDocumentParams parameters, CancellationToken cancellationToken);
}