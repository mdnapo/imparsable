using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public partial class StringLiteralExpression : LiteralExpr<string>, ISyntax, IProduction
{
    public static ISyntax Parse(ParserContext<Token> context)
    {
        var previous = context.Previous();
        return new StringLiteralExpression
        {
            Token = previous,
            Value = previous.Lexeme,
        };
    }
}