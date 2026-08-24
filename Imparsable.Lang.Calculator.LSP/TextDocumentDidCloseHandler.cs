using Imparsable.Tools.LSP.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public class TextDocumentDidCloseHandler(SyntaxBuffer buffer) : ITextDocumentDidCloseHandler
{
    public async Task HandleAsync(DidCloseTextDocumentParams parameters, CancellationToken cancellationToken) =>
        await buffer.CloseAsync(parameters.TextDocument.Uri.ToString(), cancellationToken);
}