using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public partial class IdentifierExpression : ISyntax, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }

    public static ISyntax Parse(ParserContext<Token> context) => new IdentifierExpression
    {
        Token = context.Previous()
    };
}