using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Tools.LSP.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Imparsable.Lang.Calculator.LSP;

public class CompletionHandler(SyntaxBuffer buffer) : ICompletionHandler
{
    public async Task<CompletionList> HandleAsync(CompletionParams parameters, CancellationToken cancellationToken)
    {
        var uri = parameters.TextDocument.Uri.ToString();
        var tree = await buffer.GetBufferAsync(uri, cancellationToken);
        var consts = tree.Roots
            .OfType<ConstStatement>()
            .Select(x => new CompletionItem
            {
                Label = x.Symbol,
                Kind = CompletionItemKind.Constant,
            });

        var vars = tree.Roots
            .OfType<VarStatement>()
            .Select(x => new CompletionItem
            {
                Label = x.Symbol,
                Kind = CompletionItemKind.Constant,
            });

        return new CompletionList([.. consts, .. vars]);
    }
}