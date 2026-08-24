using Imparsable.Tools.LSP;
using Imparsable.Tools.LSP.Interfaces;
using Imparsable.Lang.Calculator.LSP.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

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