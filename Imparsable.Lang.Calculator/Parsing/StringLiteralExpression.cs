using Imparsable.Tools.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class StringLiteralExpression : LiteralExpr<string>, ISyntax, IProduction
{
    public static ISyntax Parse(ParserContext<Token> context)
    {
        var previous = context.Previous();
        var text = context.Source.GetText(previous.Offset, previous.Length);
        return new StringLiteralExpression
        {
            Token = previous,
            Value = text,
        };
    }
}