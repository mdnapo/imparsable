using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Toolchain.LSP.Interfaces;

public interface ITextDocumentDidChangeHandler : ILspMethodHandler
{
    public Task HandleAsync(DidChangeTextDocumentParams parameters);
}