using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace Imparsable.LSP.Calculator;

public class TextDocumentIdentifier : ITextDocumentIdentifier
{
    public TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri) => new(uri, "clc", "clc");
}