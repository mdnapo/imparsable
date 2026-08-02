using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public class StringLiteralExpression : LiteralExpr<string>, IProduction
{
    public static ISyntax Parse(Parser<Token>.Context context)
    {
        var previous = context.Previous();
        return new StringLiteralExpression
        {
            Token = previous,
            Value = previous.Lexeme,
        };
    }
}