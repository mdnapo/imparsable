using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Toolchain.LSP.Interfaces;

public interface ITextDocumentHoverHandler : ILspMethodHandler
{
    public Hover? Handle(HoverParams parameters);
}