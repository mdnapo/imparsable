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
        Method = LspMethodName.TextDocumentDidOpen,
        RegisterOptions = new TextDocumentOpenRegistrationOptions
        {
            DocumentSelector = DocumentSelector
        }
    };

    private static readonly Registration TextDocumentDidChange = new()
    {
        Id = "calculator-document-change",
        Method = LspMethodName.TextDocumentDidChange,
        RegisterOptions = new TextDocumentChangeRegistrationOptions
        {
            SyncKind = TextDocumentSyncKind.Incremental,
            DocumentSelector = DocumentSelector
        }
    };

    private static readonly Registration TextDocumentDidClose = new()
    {
        Id = "calculator-document-close",
        Method = LspMethodName.TextDocumentDidClose,
        RegisterOptions = new TextDocumentCloseRegistrationOptions
        {
            DocumentSelector = DocumentSelector
        }
    };

    private static readonly Registration TextDocumentCompletion = new()
    {
        Id = "calculator-completion",
        Method = LspMethodName.TextDocumentCompletion,
        RegisterOptions = new CompletionRegistrationOptions
        {
            DocumentSelector = DocumentSelector,
            ResolveProvider = false,
            TriggerCharacters = new Container<string>("+", "-", "*", "/", "%")
        }
    };

    private static readonly Registration TextDocumentFormatting = new()
    {
        Id = "calculator-document-formatting",
        Method = LspMethodName.TextDocumentFormatting,
        RegisterOptions = new DocumentFormattingRegistrationOptions
        {
            DocumentSelector = DocumentSelector
        }
    };

    private static readonly Registration TextDocumentHover = new()
    {
        Id = "calculator-hover",
        Method = LspMethodName.TextDocumentHover,
        RegisterOptions = new HoverRegistrationOptions
        {
            DocumentSelector = DocumentSelector,
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
                    TextDocumentCompletion,
                    TextDocumentFormatting,
                    TextDocumentHover
                )
            }
        );
    }
}