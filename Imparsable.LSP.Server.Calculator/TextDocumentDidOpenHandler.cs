using Imparsable.LSP.Protocol.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Server.Calculator;

public class TextDocumentDidOpenHandler(SyntaxBuffer buffer) : ITextDocumentDidOpenHandler
{
    public async Task HandleAsync(DidOpenTextDocumentParams parameters, CancellationToken cancellationToken) =>
        await buffer.OpenAsync(parameters.TextDocument.Uri.ToString(), parameters.TextDocument.Text, cancellationToken);
}