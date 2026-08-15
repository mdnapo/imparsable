using Imparsable.LSP.Protocol.Attributes;
using Imparsable.LSP.Protocol.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Protocol;

public abstract class LanguageServer(JsonRpcProvider rpc, IEnumerable<ILspMethodHandler> handlers)
{
    public async Task ConnectAsync() => await rpc.ConnectAsync(this);

    private T RequireHandler<T>() where T : ILspMethodHandler =>
        handlers.OfType<T>().FirstOrDefault() ??
        throw new InvalidOperationException($"Handler of type {typeof(T)} not found");

    [LspMethod("initialize")]
    public InitializeResult Initialize(InitializeParams parameters) =>
        RequireHandler<IInitializeHandler>().Handle(parameters);

    [LspMethod("initialized")]
    public Task Initialized(CancellationToken cancellationToken) =>
        RequireHandler<IInitializedHandler>().HandleAsync(cancellationToken);

    [LspMethod("textDocument/didOpen")]
    public async Task DidOpen(DidOpenTextDocumentParams parameters, CancellationToken cancellationToken) =>
        await RequireHandler<ITextDocumentDidOpenHandler>().HandleAsync(parameters, cancellationToken);

    [LspMethod("textDocument/didChange")]
    public async Task DidChange(DidChangeTextDocumentParams parameters, CancellationToken cancellationToken) =>
        await RequireHandler<ITextDocumentDidChangeHandler>().HandleAsync(parameters, cancellationToken);

    [LspMethod("textDocument/didClose")]
    public async Task DidClose(DidCloseTextDocumentParams parameters, CancellationToken cancellationToken) =>
        await RequireHandler<ITextDocumentDidCloseHandler>().HandleAsync(parameters, cancellationToken);

    [LspMethod("textDocument/completion")]
    public async Task Completion(CompletionParams parameters, CancellationToken cancellationToken) =>
        await RequireHandler<ICompletionHandler>().HandleAsync(parameters, cancellationToken);
}