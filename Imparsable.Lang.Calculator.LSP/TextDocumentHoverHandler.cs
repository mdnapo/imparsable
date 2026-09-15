using Imparsable.Lang.Calculator.LSP.Extensions;
using Imparsable.Toolchain.LSP.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public class TextDocumentHoverHandler(SyntaxBuffer buffer) : ITextDocumentHoverHandler
{
    public Hover? Handle(HoverParams parameters)
    {
        if (!parameters.TextDocument.IsCalculatorDocument())
            return null;

        var tree = buffer.GetBufferAsync(parameters.TextDocument.Uri.ToString());

        return HoverProvider.Execute(tree, parameters.Position);
    }
}