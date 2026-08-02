using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public class IdentifierExpression : ISyntax, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }

    public static ISyntax Parse(Parser<Token>.Context context) => new IdentifierExpression
    {
        Token = context.Previous()
    };
}