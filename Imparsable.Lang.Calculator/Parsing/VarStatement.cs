using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class VarStatement : ISyntax, ISymbol, IProduction
{
    public required Source Source { get; init; }
    public required Lexer<Token>.Token Token { get; init; }
    public required Lexer<Token>.Token Identifier { get; init; }
    public Lexer<Token>.Token? Assignment { get; init; }
    public ISyntax? Initializer { get; init; }
    public required Lexer<Token>.Token SemiColon { get; init; }
    public string Symbol => Source.GetText(Identifier.Offset, Identifier.Length);

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var token = context.Previous();
        var identifier = context.Consume(Parsing.Token.IDENTIFIER, "Expected an identifier.");

        if (context.Match(Parsing.Token.SEMICOLON))
        {
            return new VarStatement
            {
                Source = context.Source,
                Token = token,
                Identifier = identifier,
                SemiColon = context.Previous()
            };
        }

        var assignment = context.Consume(Parsing.Token.EQUAL, "Expected '='.");
        var initializer = Expression.Parse(context);
        var semiColon = context.Consume(Parsing.Token.SEMICOLON, "Expected ';'.");

        return new VarStatement
        {
            Source = context.Source,
            Token = token,
            Identifier = identifier,
            Assignment = assignment,
            Initializer = initializer,
            SemiColon = semiColon
        };
    }
}