using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class BreakStatement : ISyntax, IProduction
{
    public Lexer<Token>.Token Token => Keyword;
    public required Lexer<Token>.Token Keyword { get; init; }
    public required Lexer<Token>.Token SemiColon { get; init; }

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var keyword = context.Previous();
        var semiColon = context.Consume(Parsing.Token.SEMICOLON, "Expected ';'.");
        return new BreakStatement { Keyword = keyword, SemiColon = semiColon };
    }
}