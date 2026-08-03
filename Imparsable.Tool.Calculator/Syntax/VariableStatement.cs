using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public class VariableStatement : ISyntax, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }
    public required Lexer<Token>.Token Identifier { get; init; }
    public Lexer<Token>.Token? Assignment { get; init; }
    public ISyntax? Initializer { get; set; }
    public required Lexer<Token>.Token SemiColon { get; init; }

    public static ISyntax Parse(Parser<Token>.Context context)
    {
        var token = context.Previous();
        var identifier = context.Consume(Syntax.Token.IDENTIFIER, "Expected an identifier.");

        if (context.Match(Syntax.Token.SEMICOLON))
        {
            return new VariableStatement
            {
                Token = token,
                Identifier = identifier,
                SemiColon = context.Previous()
            };
        }

        var assignment = context.Consume(Syntax.Token.EQUALS, "Expected '='.");
        var initializer = ExpressionProduction.Parse(context);
        var semiColon = context.Consume(Syntax.Token.SEMICOLON, "Expected ';'.");

        return new VariableStatement
        {
            Token = token,
            Identifier = identifier,
            Assignment = assignment,
            Initializer = initializer,
            SemiColon = semiColon
        };
    }
}