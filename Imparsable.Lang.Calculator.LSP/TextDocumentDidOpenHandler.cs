using Imparsable.Toolchain.LSP;
using Imparsable.Toolchain.LSP.Interfaces;
using Imparsable.Lang.Calculator.LSP.Extensions;
using Imparsable.Toolchain;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public class TextDocumentDidOpenHandler(SyntaxBuffer buffer, JsonRpcProvider rpc) : ITextDocumentDidOpenHandler
{
    public async Task HandleAsync(DidOpenTextDocumentParams parameters)
    {
        var diagnostics = new DiagnosticsProvider();
        var uri = parameters.TextDocument.Uri.ToString();

        buffer.OpenAsync(uri, parameters.TextDocument.Text, diagnostics);

        var publishDiagnosticsParams = diagnostics.ToPublishDiagnosticsParams(uri);

        await rpc.Connection.NotifyWithParameterObjectAsync(LspMethodName.PublishDiagnostics, publishDiagnosticsParams);
    }
}