using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class IdentifierExpression : ISyntax, IProduction, ISymbol
{
    public required Source Source { get; init; }
    public required Lexer<Token>.Token Token { get; init; }
    public required string Symbol { get; init; }

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var token = context.Previous();
        var symbol = context.Source.GetText(token.Offset, token.Length);

        return new IdentifierExpression
        {
            Source = context.Source,
            Token = context.Previous(),
            Symbol = symbol
        };
    }
}