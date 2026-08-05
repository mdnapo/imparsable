using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public partial class NumericLiteralExpression : LiteralExpr<double>, ISyntax, IProduction
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