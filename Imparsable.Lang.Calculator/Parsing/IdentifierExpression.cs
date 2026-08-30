using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class IdentifierExpression : ISyntax, IProduction, ISymbol
{
    public required Source Source { get; init; }
    public required Lexer<Token>.Token Token { get; init; }
    public string Symbol => Source.GetText(Token.Offset, Token.Length);

    public static ISyntax Parse(ParserContext<Token> context) => new IdentifierExpression
    {
        Source = context.Source,
        Token = context.Previous()
    };
}