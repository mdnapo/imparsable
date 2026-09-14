using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Toolchain.LSP.Interfaces;

public interface IFormattingHandler : ILspMethodHandler
{
    public Task<TextEdit[]> HandleAsync(DocumentFormattingParams parameters);
}