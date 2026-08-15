using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.LSP.Protocol.Interfaces;

public interface IInitializeHandler : ILspMethodHandler
{
    public InitializeResult Handle(InitializeParams parameters);
}