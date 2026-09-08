using Imparsable.Toolchain.LSP;
using Imparsable.Toolchain.LSP.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Imparsable.Lang.Calculator.LSP;

public class InitializedHandler(JsonRpcProvider rpc) : IInitializedHandler
{
    private static readonly TextDocumentSelector DocumentSelector = new(new TextDocumentFilter
    {
        Language = "clc",
        Scheme = "file",
        Pattern = "**/*.clc"
    });

    private static readonly Registration TextDocumentDidOpen = new()
    {
        Id = "calculator-document-open",
        Method = "textDocument/didOpen",
        RegisterOptions = new TextDocumentOpenRegistrationOptions
        {
            DocumentSelector = DocumentSelector
        }
    };

    private static readonly Registration TextDocumentDidChange = new()
    {
        Id = "calculator-document-change",
        Method = "textDocument/didChange",
        RegisterOptions = new TextDocumentChangeRegistrationOptions
        {
            SyncKind = TextDocumentSyncKind.Incremental,
            DocumentSelector = DocumentSelector
        }
    };

    private static readonly Registration TextDocumentDidClose = new()
    {
        Id = "calculator-document-close",
        Method = "textDocument/didClose",
        RegisterOptions = new TextDocumentCloseRegistrationOptions
        {
            DocumentSelector = DocumentSelector
        }
    };

    private static readonly Registration TextDocumentCompletion = new()
    {
        Id = "calculator-completion",
        Method = "textDocument/completion",
        RegisterOptions = new CompletionRegistrationOptions
        {
            DocumentSelector = DocumentSelector,
            ResolveProvider = false,
            TriggerCharacters = new Container<string>("+", "-", "*", "/", ".")
        }
    };

    public async Task HandleAsync()
    {
        await rpc.Connection.InvokeWithParameterObjectAsync(
            LspMethodName.RegisterClientCapability,
            new RegistrationParams
            {
                Registrations = new RegistrationContainer(
                    TextDocumentDidOpen,
                    TextDocumentDidChange,
                    TextDocumentDidClose,
                    TextDocumentCompletion
                )
            }
        );
    }
}