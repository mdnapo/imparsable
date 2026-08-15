using Imparsable.LSP.Protocol.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Imparsable.LSP.Server.Calculator;

public class InitializeHandler : IInitializeHandler
{
    public InitializeResult Handle(InitializeParams parameters) => new()
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
            CompletionProvider = new CompletionRegistrationOptions.StaticOptions
            {
                ResolveProvider = false,
                TriggerCharacters = new Container<string>("+", "-", "*", "/")
            }
        },
        ServerInfo = new ServerInfo()
    };
}