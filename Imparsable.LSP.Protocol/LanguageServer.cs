using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using StreamJsonRpc;

namespace Imparsable.LSP.Protocol;

public abstract class LanguageServer(IHttpContextAccessor httpContextAccessor, ISourceTextBuffer buffer) : IDisposable
{
    private JsonRpc? Rpc { get; set; }

    public async Task ConnectAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (!httpContext.WebSockets.IsWebSocketRequest)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using var socket = await httpContext.WebSockets.AcceptWebSocketAsync();
        await using var handler = new WebSocketMessageHandler(socket, GetFormatter());

        Rpc = new JsonRpc(handler);
        Rpc.AddLocalRpcTarget(this);
        Rpc.StartListening();

        await Rpc.Completion.WaitAsync(httpContext.RequestAborted);
    }

    private static JsonMessageFormatter GetFormatter() => new()
    {
        JsonSerializer = { ContractResolver = new CamelCasePropertyNamesContractResolver() }
    };

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
    public async Task DidChange(DidChangeTextDocumentParams parameters, CancellationToken cancellationToken) =>
        await buffer.UpdateAsync(
            parameters.TextDocument.Uri.ToString(),
            parameters.ContentChanges.First().Text,
            cancellationToken);

    [LspMethod("textDocument/didClose")]
    public async Task DidClose(DidCloseTextDocumentParams parameters, CancellationToken cancellationToken) =>
        await buffer.CloseAsync(parameters.TextDocument.Uri.ToString(), cancellationToken);

    public void Dispose() => Rpc?.Dispose();
}