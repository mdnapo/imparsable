using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public class ConstStatement : ISyntax, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }
    public required Lexer<Token>.Token Identifier { get; init; }
    public required Lexer<Token>.Token Assignment { get; init; }
    public required ISyntax Initializer { get; init; }
    public required Lexer<Token>.Token SemiColon { get; init; }

    public static ISyntax Parse(Parser<Token>.Context context)
    {
        var token = context.Previous();
        var identifier = context.Consume(Calculator.Token.IDENTIFIER, "Expected an identifier.");
        var assignment = context.Consume(Calculator.Token.EQUALS, "Expected '='.");
        var initializer = ExpressionProduction.Parse(context);
        var semiColon = context.Consume(Calculator.Token.SEMICOLON, "Expected ';'.");

        return new ConstStatement
        {
            Token = token,
            Identifier = identifier,
            Assignment = assignment,
            Initializer = initializer,
            SemiColon = semiColon
        };
    }
}