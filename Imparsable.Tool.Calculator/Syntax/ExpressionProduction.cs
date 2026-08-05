using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public  abstract class ExpressionProduction : IProduction
{
    public static ISyntax Parse(Parser<Token>.Context context)
    {
        return BinaryExpression.Parse(context);
    }
}