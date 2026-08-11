using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace Imparsable.LSP.Calculator;

public class DidChangeTextDocumentHandler(SourceBuffer sources, ILanguageServerFacade languageServer)
    : IDidChangeTextDocumentHandler
{
    public Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
    {
        var documentPath = request.TextDocument.Uri.ToString();
        var text = request.ContentChanges.FirstOrDefault()?.Text;

        ArgumentNullException.ThrowIfNull(text);

        sources[documentPath] = text;

        languageServer.Window.LogInfo($"Updated buffer for document: {documentPath}\n{text}");

        return Unit.Task;
    }

    public TextDocumentChangeRegistrationOptions GetRegistrationOptions(
        TextSynchronizationCapability capability,
        ClientCapabilities clientCapabilities
    ) => new()
    {
        SyncKind = TextDocumentSyncKind.Full,
        DocumentSelector = Defaults.DocumentSelector
    };
}