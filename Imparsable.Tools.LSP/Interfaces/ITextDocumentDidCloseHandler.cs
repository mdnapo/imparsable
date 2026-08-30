using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Tools.LSP.Interfaces;

public interface ITextDocumentDidCloseHandler : ILspMethodHandler
{
    public void Handle(DidCloseTextDocumentParams parameters);
}