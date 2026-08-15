using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Protocol.Interfaces;

public interface ITextDocumentDidOpenHandler : ILspMethodHandler
{
    public Task HandleAsync(DidOpenTextDocumentParams parameters, CancellationToken cancellationToken);
}