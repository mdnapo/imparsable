using Imparsable.Toolchain.LSP;
using Imparsable.Toolchain.LSP.Interfaces;
using Imparsable.Lang.Calculator.LSP.Extensions;
using Imparsable.Toolchain;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public class TextDocumentDidChangeHandler(SyntaxBuffer buffer, JsonRpcProvider rpc) : ITextDocumentDidChangeHandler
{
    public async Task HandleAsync(DidChangeTextDocumentParams parameters)
    {
        var diagnostics = new DiagnosticsProvider();
        var uri = parameters.TextDocument.Uri.ToString();

        // TODO: Implement incremental updates
        buffer.UpdateAsync(uri, parameters.ContentChanges.First().Text, diagnostics);
        
        var publishDiagnosticsParams = diagnostics.ToPublishDiagnosticsParams(uri);

        await rpc.Connection.NotifyWithParameterObjectAsync(LspMethodName.PublishDiagnostics, publishDiagnosticsParams);
    }
}