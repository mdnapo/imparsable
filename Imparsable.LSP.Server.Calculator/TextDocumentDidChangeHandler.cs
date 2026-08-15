using Imparsable.LSP.Protocol.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Server.Calculator;

public class TextDocumentDidChangeHandler(SyntaxBuffer buffer) : ITextDocumentDidChangeHandler
{
    public async Task HandleAsync(DidChangeTextDocumentParams parameters, CancellationToken cancellationToken) =>
        await buffer.UpdateAsync(
            parameters.TextDocument.Uri.ToString(),
            parameters.ContentChanges.First().Text,
            cancellationToken);
}