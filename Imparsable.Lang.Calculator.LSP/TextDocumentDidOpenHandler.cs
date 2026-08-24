using Imparsable.Tools.LSP;
using Imparsable.Tools.LSP.Interfaces;
using Imparsable.Lang.Calculator.LSP.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public class TextDocumentDidOpenHandler(SyntaxBuffer buffer, JsonRpcProvider rpc) : ITextDocumentDidOpenHandler
{
    public async Task HandleAsync(DidOpenTextDocumentParams parameters, CancellationToken cancellationToken)
    {
        var uri = parameters.TextDocument.Uri.ToString();
        await buffer.OpenAsync(uri, parameters.TextDocument.Text, cancellationToken);
        var tree = await buffer.GetBufferAsync(uri, cancellationToken);
        var diagnostics = tree.ToPublishDiagnosticsParams(uri);

        await rpc.Connection.NotifyWithParameterObjectAsync(LspMethodName.PublishDiagnostics, diagnostics);
    }
}