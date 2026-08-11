using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Imparsable.LSP.Calculator;

public class CompletionHandler : ICompletionHandler
{
    public Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CompletionList(new CompletionItem
        {
            Label = "pi",
            Kind = CompletionItemKind.Reference,
            TextEdit = new TextEdit
            {
                NewText = "pi",
                Range = new Range(
                    new Position
                    {
                        Line = request.Position.Line,
                        Character = request.Position.Character + 1
                    }, new Position
                    {
                        Line = request.Position.Line,
                        Character = request.Position.Character + 2 - 1
                    })
            }
        }));
    }

    public CompletionRegistrationOptions GetRegistrationOptions(
        CompletionCapability capability,
        ClientCapabilities clientCapabilities
    ) => new()
    {
        DocumentSelector = Defaults.DocumentSelector,
        ResolveProvider = false
    };
}