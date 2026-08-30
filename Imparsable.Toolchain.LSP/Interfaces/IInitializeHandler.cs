using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Toolchain.LSP.Interfaces;

public interface IInitializeHandler : ILspMethodHandler
{
    public InitializeResult Handle(InitializeParams parameters);
}