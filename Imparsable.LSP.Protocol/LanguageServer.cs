using Imparsable.LSP.Protocol.Attributes;
using Imparsable.LSP.Protocol.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Imparsable.LSP.Protocol;

public abstract class LanguageServer(
    JsonRpcProvider rpc,
    ISourceTextBuffer buffer,
    IEnumerable<ILspMethodHandler> handlers
) 
{
    private readonly ICompletionHandler? _completion = handlers.OfType<ICompletionHandler>().FirstOrDefault();

    public async Task ConnectAsync() => await rpc.ConnectAsync(this);

    [LspMethod("initialize")]
    public InitializeResult Initialize(InitializeParams parameters) => new()
    {
        Capabilities = new ServerCapabilities
        {
            TextDocumentSync = new TextDocumentSync(TextDocumentSyncKind.Full)
            {
                Options = new()
                {
                    Change = TextDocumentSyncKind.Full,
                    OpenClose = true,
                }
            },
        },
        ServerInfo = new ServerInfo()
    };

    [LspMethod("initialized")]
    public Task Initialized() => Task.CompletedTask;

    [LspMethod("textDocument/didOpen")]
    public async Task DidOpen(DidOpenTextDocumentParams parameters, CancellationToken cancellationToken) =>
        await buffer.OpenAsync(parameters.TextDocument.Uri.ToString(), parameters.TextDocument.Text, cancellationToken);

    [LspMethod("textDocument/didChange")]
    public async Task DidChange(DidChangeTextDocumentParams parameters, CancellationToken cancellationToken)
    {
        await buffer.UpdateAsync(
            parameters.TextDocument.Uri.ToString(),
            parameters.ContentChanges.First().Text,
            cancellationToken);
    }

    [LspMethod("textDocument/didClose")]
    public async Task DidClose(DidCloseTextDocumentParams parameters, CancellationToken cancellationToken) =>
        await buffer.CloseAsync(parameters.TextDocument.Uri.ToString(), cancellationToken);

    [LspMethod("textDocument/completion")]
    public async Task Completion(CompletionParams parameters, CancellationToken cancellationToken) =>
        await (_completion?.CompleteAsync(parameters, cancellationToken) ?? Task.CompletedTask);
}