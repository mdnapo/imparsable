using Imparsable.Lang.Calculator.LSP.Extensions;
using Imparsable.Toolchain.LSP.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public class CompletionHandler(SyntaxBuffer buffer) : ICompletionHandler
{
    public CompletionList Handle(CompletionParams parameters)
    {
        if (!parameters.TextDocument.IsCalculatorDocument())
            return [];

        var uri = parameters.TextDocument.Uri.ToString();
        var tree = buffer.GetBufferAsync(uri);
        return CompletionItemProvider.Execute(tree, parameters.Position);
    }
}