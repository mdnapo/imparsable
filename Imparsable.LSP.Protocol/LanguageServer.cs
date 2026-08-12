using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using StreamJsonRpc;

namespace Imparsable.LSP.Protocol;

public abstract class LanguageServer(ISourceTextBuffer buffer)
{
    protected JsonRpc? Rpc { get; set; }

    public void Attach(JsonRpc rpc) => Rpc = rpc;

    [LspMethod("initialize")]
    public InitializeResult Initialize(InitializeParams parameters) => new()
    {
        Capabilities = new ServerCapabilities
        {
            TextDocumentSync = new TextDocumentSync(TextDocumentSyncKind.Full),
            // CompletionProvider = new CompletionRegistrationOptions.StaticOptions
            // {
            //     ResolveProvider = false
            // }
        },
        ServerInfo = new ServerInfo()
    };

    [LspMethod("initialized")]
    public Task Initialized() => Task.CompletedTask;

    [LspMethod("textDocument/didOpen")]
    public async Task DidOpen(DidOpenTextDocumentParams parameters, CancellationToken cancellationToken) =>
        await buffer.OpenAsync(parameters.TextDocument.Uri.ToString(), parameters.TextDocument.Text, cancellationToken);

    [LspMethod("textDocument/didChange")]
    public async Task DidChange(DidChangeTextDocumentParams parameters, CancellationToken cancellationToken) =>
        await buffer.UpdateAsync(
            parameters.TextDocument.Uri.ToString(),
            parameters.ContentChanges.First().Text,
            cancellationToken);

    [LspMethod("textDocument/didClose")]
    public async Task DidClose(DidCloseTextDocumentParams parameters, CancellationToken cancellationToken) =>
        await buffer.CloseAsync(parameters.TextDocument.Uri.ToString(), cancellationToken);
}