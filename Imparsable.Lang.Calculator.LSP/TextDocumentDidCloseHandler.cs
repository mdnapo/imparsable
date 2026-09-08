using Imparsable.Toolchain.LSP.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public class TextDocumentDidCloseHandler(SyntaxBuffer buffer) : ITextDocumentDidCloseHandler
{
    public void Handle(DidCloseTextDocumentParams parameters)
    {
        if (!IsCalculatorDocument(parameters.TextDocument))
            return;
        
        buffer.CloseAsync(parameters.TextDocument.Uri.ToString());
    }
    
    private static bool IsCalculatorDocument(TextDocumentIdentifier document) =>
        document.Uri.Scheme == "file" && document.Uri.Path.EndsWith(Constants.FileExtension, StringComparison.OrdinalIgnoreCase);
}