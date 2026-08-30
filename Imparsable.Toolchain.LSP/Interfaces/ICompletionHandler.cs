using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Toolchain.LSP.Interfaces;

public interface ICompletionHandler : ILspMethodHandler
{
    public CompletionList Handle(CompletionParams parameters);
}