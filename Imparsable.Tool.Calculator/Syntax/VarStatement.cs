using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public partial class VarStatement : ISyntax, ISymbol, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }
    public required Lexer<Token>.Token Identifier { get; init; }
    public Lexer<Token>.Token? Assignment { get; init; }
    public ISyntax? Initializer { get; init; }
    public required Lexer<Token>.Token SemiColon { get; init; }
    public string Symbol => Identifier.Lexeme;

    public static ISyntax Parse(Parser<Token>.Context context)
    {
        var token = context.Previous();
        var identifier = context.Consume(Syntax.Token.IDENTIFIER, "Expected an identifier.");

        if (context.Match(Syntax.Token.SEMICOLON))
        {
            return new VarStatement
            {
                Token = token,
                Identifier = identifier,
                SemiColon = context.Previous()
            };
        }

        var assignment = context.Consume(Syntax.Token.EQUALS, "Expected '='.");
        var initializer = ExpressionProduction.Parse(context);
        var semiColon = context.Consume(Syntax.Token.SEMICOLON, "Expected ';'.");

        return new VarStatement
        {
            Token = token,
            Identifier = identifier,
            Assignment = assignment,
            Initializer = initializer,
            SemiColon = semiColon
        };
    }
}