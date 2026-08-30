using Imparsable.Tools.LSP.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public class TextDocumentDidCloseHandler(SyntaxBuffer buffer) : ITextDocumentDidCloseHandler
{
    public void Handle(DidCloseTextDocumentParams parameters) =>
        buffer.CloseAsync(parameters.TextDocument.Uri.ToString());
}