using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Tools.LSP.Interfaces;

public interface IInitializeHandler : ILspMethodHandler
{
    public InitializeResult Handle(InitializeParams parameters);
}