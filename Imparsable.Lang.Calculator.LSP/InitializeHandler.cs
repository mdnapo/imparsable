using Imparsable.Toolchain.LSP.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Imparsable.Lang.Calculator.LSP;

public class InitializeHandler : IInitializeHandler
{
    public InitializeResult Handle(InitializeParams parameters) => new()
    {
        ServerInfo = new ServerInfo(),
        Capabilities = new ServerCapabilities()
    };
}