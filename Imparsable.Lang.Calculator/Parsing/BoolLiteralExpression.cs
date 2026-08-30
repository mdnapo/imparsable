using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class BoolLiteralExpression : LiteralExpr<bool>, ISyntax, IProduction
{
    public static ISyntax Parse(ParserContext<Token> context)
    {
        var previous = context.Previous();
        var text = context.Source.GetText(previous.Offset, previous.Length);
        return new BoolLiteralExpression
        {
            Token = previous,
            Value = bool.Parse(text),
        };
    }
}