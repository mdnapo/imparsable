using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Toolchain.LSP.Interfaces;

public interface ITextDocumentCompletionHandler : ILspMethodHandler
{
    public CompletionList Handle(CompletionParams parameters);
}