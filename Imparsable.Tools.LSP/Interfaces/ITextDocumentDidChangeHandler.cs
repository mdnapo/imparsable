using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Tools.LSP.Interfaces;

public interface ITextDocumentDidChangeHandler : ILspMethodHandler
{
    public Task HandleAsync(DidChangeTextDocumentParams parameters);
}