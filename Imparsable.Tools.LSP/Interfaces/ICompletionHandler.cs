using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Tools.LSP.Interfaces;

public interface ICompletionHandler : ILspMethodHandler
{
    public CompletionList Handle(CompletionParams parameters);
}