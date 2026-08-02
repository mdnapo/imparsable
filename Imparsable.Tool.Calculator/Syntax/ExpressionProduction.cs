using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public abstract class ExpressionProduction : IProduction
{
    public static ISyntax Parse(Parser<Token>.Context context)
    {
        if (context.Match(Token.IDENTIFIER))
            return IdentifierExpression.Parse(context);

        if (context.Match(Token.LEFT_PARENTHESIS))
            return GroupingExpression.Parse(context);

        if (context.Match(Token.STRING))
            return StringLiteralExpression.Parse(context);

        if (context.Match(Token.NUMBER))
            return NumericLiteralExpression.Parse(context);

        return BinaryExpression.Parse(context);
    }
}