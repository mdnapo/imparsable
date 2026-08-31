using Imparsable.Toolchain.LSP.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public class CompletionHandler(SyntaxBuffer buffer) : ICompletionHandler
{
    public CompletionList Handle(CompletionParams parameters)
    {
        var uri = parameters.TextDocument.Uri.ToString();
        var tree = buffer.GetBufferAsync(uri);
        return CompletionWalker.Execute(tree, parameters.Position);
    }
}