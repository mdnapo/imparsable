using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public partial class PrintStatement : ISyntax, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }
    public required ISyntax Expression { get; init; }
    public required Lexer<Token>.Token SemiColon { get; init; }

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var token = context.Previous();
        var expression = ExpressionProduction.Parse(context);
        var semiColon = context.Consume(Syntax.Token.SEMICOLON, "Expected ';'.");

        return new PrintStatement
        {
            Token = token,
            Expression = expression,
            SemiColon = semiColon
        };
    }
}