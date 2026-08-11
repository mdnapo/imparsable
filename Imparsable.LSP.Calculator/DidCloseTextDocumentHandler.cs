using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Calculator;

public class DidCloseTextDocumentHandler(SourceBuffer sources) : IDidCloseTextDocumentHandler
{
    public Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
    {
        sources.Remove(request.TextDocument.Uri.ToString());
        return Unit.Task;
    }

    public TextDocumentCloseRegistrationOptions GetRegistrationOptions(
        TextSynchronizationCapability capability,
        ClientCapabilities clientCapabilities
    ) => new() { DocumentSelector = Defaults.DocumentSelector };
}