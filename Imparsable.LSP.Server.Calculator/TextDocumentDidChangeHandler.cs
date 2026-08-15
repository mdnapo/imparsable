using Imparsable.LSP.Protocol;
using Imparsable.LSP.Protocol.Interfaces;
using Imparsable.LSP.Server.Calculator.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Server.Calculator;

public class TextDocumentDidChangeHandler(SyntaxBuffer buffer, JsonRpcProvider rpc) : ITextDocumentDidChangeHandler
{
    public async Task HandleAsync(DidChangeTextDocumentParams parameters, CancellationToken cancellationToken)
    {
        var uri = parameters.TextDocument.Uri.ToString();
        // TODO: Implement incremental updates
        await buffer.UpdateAsync(uri, parameters.ContentChanges.First().Text, cancellationToken);
        var tree = await buffer.GetBufferAsync(uri, cancellationToken);
        var diagnostics = tree.ToPublishDiagnosticsParams(uri);

        await rpc.Connection.NotifyWithParameterObjectAsync(LspMethodName.PublishDiagnostics, diagnostics);
    }
}