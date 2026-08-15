using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Protocol.Interfaces;

public interface ITextDocumentDidChangeHandler : ILspMethodHandler
{
    public Task HandleAsync(DidChangeTextDocumentParams parameters, CancellationToken cancellationToken);
}