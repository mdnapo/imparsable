using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Toolchain.LSP.Interfaces;

public interface ITextDocumentDidOpenHandler : ILspMethodHandler
{
    public Task HandleAsync(DidOpenTextDocumentParams parameters);
}