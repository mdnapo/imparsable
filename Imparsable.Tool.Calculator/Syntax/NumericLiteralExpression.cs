using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public class NumericLiteralExpression : LiteralExpr<double>, IProduction
{
    public static ISyntax Parse(Parser<Token>.Context context)
    {
        var previous = context.Previous();
        return new NumericLiteralExpression
        {
            Token = previous,
            Value = double.Parse(previous.Lexeme),
        };
    }
}