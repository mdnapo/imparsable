using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public abstract class Expression : IProduction
{
    public static ISyntax Parse(ParserContext<Token> context)
    {
        return AssignmentExpression.Parse(context);
    }
}