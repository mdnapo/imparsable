using System.Text.RegularExpressions;
using Imparsable.Lang.Calculator.Tools;
using Imparsable.Toolchain.LSP.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Imparsable.Lang.Calculator.LSP;

public class FormattingHandler(SyntaxBuffer buffer) : IFormattingHandler
{
    public Task<TextEdit[]> HandleAsync(DocumentFormattingParams parameters)
    {
        if (!IsCalculatorDocument(parameters.TextDocument))
            return Task.FromResult(Array.Empty<TextEdit>());

        var document = parameters.TextDocument;
        var tree = buffer.GetBufferAsync(document.Uri.ToString());
        var formatted = Formatter.Format(tree, parameters.Options.TabSize);

        if (formatted == tree.Source.Text)
            return Task.FromResult(Array.Empty<TextEdit>());

        var end = Regex.Count(tree.Source.Text, $"{Environment.NewLine}") - 1;

        return Task.FromResult(new TextEdit[]
        {
            new()
            {
                Range = new Range(
                    startLine: 0,
                    startCharacter: 0,
                    endLine: Regex.Count(tree.Source.Text, $"{Environment.NewLine}") - 1,
                    endCharacter: tree.Source.Text.Length - 1
                ),
                NewText = formatted,
            }
        });
    }

    private static bool IsCalculatorDocument(TextDocumentIdentifier document) =>
        document.Uri.Scheme == "file" && document.Uri.Path.EndsWith(Constants.FileExtension, StringComparison.OrdinalIgnoreCase);
}