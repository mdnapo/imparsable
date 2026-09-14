using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Toolchain.LSP.Interfaces;

public interface ITextDocumentFormattingHandler : ILspMethodHandler
{
    public Task<TextEdit[]> HandleAsync(DocumentFormattingParams parameters);
}