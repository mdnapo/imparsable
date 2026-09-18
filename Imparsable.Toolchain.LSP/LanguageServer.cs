using Imparsable.Toolchain.LSP.Attributes;
using Imparsable.Toolchain.LSP.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Toolchain.LSP;

public abstract class LanguageServer(JsonRpcProvider rpc, IEnumerable<ILspMethodHandler> handlers)
{
    public async Task ConnectAsync() => await rpc.ConnectAsync(this);

    private T RequireHandler<T>() where T : ILspMethodHandler =>
        handlers.OfType<T>().FirstOrDefault() ??
        throw new InvalidOperationException($"Handler of type {typeof(T)} not found");

    [LspMethod(LspMethodName.Initialize)]
    public InitializeResult Initialize(InitializeParams parameters) =>
        LspTelemetry.Invoke(
            LspMethodName.Initialize,
            rpc.ConnectionId,
            () => RequireHandler<IInitializeHandler>().Handle(parameters));

    [LspMethod(LspMethodName.Initialized)]
    public async Task Initialized() =>
        await LspTelemetry.InvokeAsync(
            LspMethodName.Initialized,
            rpc.ConnectionId,
            () => RequireHandler<IInitializedHandler>().HandleAsync());

    [LspMethod(LspMethodName.TextDocumentDidOpen)]
    public async Task DidOpen(DidOpenTextDocumentParams parameters) =>
        await LspTelemetry.InvokeAsync(
            LspMethodName.TextDocumentDidOpen,
            rpc.ConnectionId,
            () => RequireHandler<ITextDocumentDidOpenHandler>().HandleAsync(parameters));

    [LspMethod(LspMethodName.TextDocumentDidChange)]
    public async Task DidChange(DidChangeTextDocumentParams parameters) =>
        await LspTelemetry.InvokeAsync(
            LspMethodName.TextDocumentDidChange,
            rpc.ConnectionId,
            () => RequireHandler<ITextDocumentDidChangeHandler>().HandleAsync(parameters));

    [LspMethod(LspMethodName.TextDocumentDidClose)]
    public void DidClose(DidCloseTextDocumentParams parameters) =>
        LspTelemetry.Invoke(
            LspMethodName.TextDocumentDidClose,
            rpc.ConnectionId,
            () => RequireHandler<ITextDocumentDidCloseHandler>().Handle(parameters));

    [LspMethod(LspMethodName.TextDocumentCompletion)]
    public CompletionList Completion(CompletionParams parameters) =>
        LspTelemetry.Invoke(
            LspMethodName.TextDocumentCompletion,
            rpc.ConnectionId,
            () => RequireHandler<ITextDocumentCompletionHandler>().Handle(parameters));

    [LspMethod(LspMethodName.TextDocumentFormatting)]
    public TextEdit[] Formatting(DocumentFormattingParams parameters) =>
        LspTelemetry.Invoke(
            LspMethodName.TextDocumentFormatting,
            rpc.ConnectionId,
            () => RequireHandler<ITextDocumentFormattingHandler>().Handle(parameters));

    [LspMethod(LspMethodName.TextDocumentHover)]
    public Hover? Hover(HoverParams parameters) =>
        LspTelemetry.Invoke(
            LspMethodName.TextDocumentHover,
            rpc.ConnectionId,
            () => RequireHandler<ITextDocumentHoverHandler>().Handle(parameters));
}