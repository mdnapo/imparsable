using Imparsable.LSP.Protocol;
using Imparsable.LSP.Protocol.Interfaces;
using Imparsable.LSP.Server.Calculator.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Server.Calculator;

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