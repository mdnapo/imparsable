using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class IdentifierExpression : ISyntax, IProduction, ISymbol
{
    public required Lexer<Token>.Token Token { get; init; }
    public Lexer<Token>.Token Symbol => Token;

    public static ISyntax Parse(ParserContext<Token> context) => new IdentifierExpression
    {
        Token = context.Previous(),
    };
}