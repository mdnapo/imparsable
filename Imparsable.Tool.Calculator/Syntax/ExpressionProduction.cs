using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public  abstract class ExpressionProduction : IProduction
{
    public static ISyntax Parse(ParserContext<Token> context)
    {
        return BinaryExpression.Parse(context);
    }
}