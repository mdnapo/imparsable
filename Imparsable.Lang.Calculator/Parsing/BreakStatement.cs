using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class BreakStatement : ISyntax, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }
    public required Lexer<Token>.Token SemiColon { get; init; }

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var token = context.Previous();
        var semiColon = context.Consume(Parsing.Token.SEMICOLON, "Expected ';'.");
        return new BreakStatement { Token = token, SemiColon = semiColon };
    }
}