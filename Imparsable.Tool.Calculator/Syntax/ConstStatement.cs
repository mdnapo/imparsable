using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public partial class ConstStatement : ISyntax, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }
    public required Lexer<Token>.Token Identifier { get; init; }
    public required Lexer<Token>.Token Assignment { get; init; }
    public required ISyntax Initializer { get; init; }
    public required Lexer<Token>.Token SemiColon { get; init; }

    public static ISyntax Parse(Parser<Token>.Context context)
    {
        var token = context.Previous();
        var identifier = context.Consume(Syntax.Token.IDENTIFIER, "Expected an identifier.");
        var assignment = context.Consume(Syntax.Token.EQUALS, "Expected '='.");
        var initializer = ExpressionProduction.Parse(context);
        var semiColon = context.Consume(Syntax.Token.SEMICOLON, "Expected ';'.");

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