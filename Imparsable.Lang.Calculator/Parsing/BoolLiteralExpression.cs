using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class BoolLiteralExpression : LiteralExpr<bool>, ISyntax, IProduction
{
    public static ISyntax Parse(ParserContext<Token> context)
    {
        var previous = context.Previous();
        return new BoolLiteralExpression
        {
            Token = previous,
            Value = previous.Type == Parsing.Token.TRUE,
        };
    }
}