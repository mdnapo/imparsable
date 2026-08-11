using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Calculator;

public class DidSaveTextDocumentHandler : IDidSaveTextDocumentHandler
{
    public Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken) => Unit.Task;

    public TextDocumentSaveRegistrationOptions GetRegistrationOptions(
        TextSynchronizationCapability capability,
        ClientCapabilities clientCapabilities
    ) => new() { DocumentSelector = Defaults.DocumentSelector };
}