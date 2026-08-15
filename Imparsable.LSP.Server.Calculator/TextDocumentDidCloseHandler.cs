using Imparsable.LSP.Protocol.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Server.Calculator;

public class TextDocumentDidCloseHandler(SyntaxBuffer buffer) : ITextDocumentDidCloseHandler
{
    public async Task HandleAsync(DidCloseTextDocumentParams parameters, CancellationToken cancellationToken) =>
        await buffer.CloseAsync(parameters.TextDocument.Uri.ToString(), cancellationToken);
}