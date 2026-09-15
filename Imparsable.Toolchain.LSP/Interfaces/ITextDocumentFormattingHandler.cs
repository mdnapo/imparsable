using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Toolchain.LSP.Interfaces;

public interface ITextDocumentFormattingHandler : ILspMethodHandler
{
    public TextEdit[] Handle(DocumentFormattingParams parameters);
}