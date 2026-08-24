using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Tools.LSP.Interfaces;

public interface ITextDocumentDidOpenHandler : ILspMethodHandler
{
    public Task HandleAsync(DidOpenTextDocumentParams parameters, CancellationToken cancellationToken);
}