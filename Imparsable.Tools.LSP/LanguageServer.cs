using Imparsable.Tools.LSP.Attributes;
using Imparsable.Tools.LSP.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Tools.LSP;

public abstract class LanguageServer(JsonRpcProvider rpc, IEnumerable<ILspMethodHandler> handlers)
{
    public async Task ConnectAsync() => await rpc.ConnectAsync(this);

    private T RequireHandler<T>() where T : ILspMethodHandler =>
        handlers.OfType<T>().FirstOrDefault() ??
        throw new InvalidOperationException($"Handler of type {typeof(T)} not found");

    [LspMethod(LspMethodName.Initialize)]
    public InitializeResult Initialize(InitializeParams parameters) =>
        RequireHandler<IInitializeHandler>().Handle(parameters);

    [LspMethod(LspMethodName.Initialized)]
    public void Initialized() =>
        RequireHandler<IInitializedHandler>().Handle();

    [LspMethod(LspMethodName.TextDocumentDidOpen)]
    public async Task DidOpen(DidOpenTextDocumentParams parameters) =>
        await RequireHandler<ITextDocumentDidOpenHandler>().HandleAsync(parameters);

    [LspMethod(LspMethodName.TextDocumentDidChange)]
    public async Task DidChange(DidChangeTextDocumentParams parameters) =>
        await RequireHandler<ITextDocumentDidChangeHandler>().HandleAsync(parameters);

    [LspMethod(LspMethodName.TextDocumentDidClose)]
    public void DidClose(DidCloseTextDocumentParams parameters) =>
        RequireHandler<ITextDocumentDidCloseHandler>().Handle(parameters);

    [LspMethod(LspMethodName.TextDocumentCompletion)]
    public CompletionList Completion(CompletionParams parameters) =>
        RequireHandler<ICompletionHandler>().Handle(parameters);
}