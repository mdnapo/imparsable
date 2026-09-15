using System.Text.RegularExpressions;
using Imparsable.Lang.Calculator.LSP.Extensions;
using Imparsable.Lang.Calculator.Tools;
using Imparsable.Toolchain.LSP.Interfaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Imparsable.Lang.Calculator.LSP;

public class TextDocumentFormattingHandler(SyntaxBuffer buffer) : ITextDocumentFormattingHandler
{
    public TextEdit[] Handle(DocumentFormattingParams parameters)
    {
        if (!parameters.TextDocument.IsCalculatorDocument())
            return [];

        var document = parameters.TextDocument;
        var tree = buffer.GetBufferAsync(document.Uri.ToString());
        var formatted = Formatter.Format(tree, parameters.Options.TabSize);

        if (formatted == tree.Source.Text)
            return [];

        var edit = new TextEdit
        {
            Range = new Range(
                startLine: 0,
                startCharacter: 0,
                endLine: Regex.Count(tree.Source.Text, $"{Environment.NewLine}"),
                endCharacter: tree.Source.Text.Length
            ),
            NewText = formatted,
        };

        return [edit];
    }
}