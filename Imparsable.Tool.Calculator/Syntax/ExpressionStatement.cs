using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public class ExpressionStatement : ISyntax, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }
    public required ISyntax Expr { get; init; }
    public required Lexer<Token>.Token SemiColon { get; set; }

    public static ISyntax Parse(Parser<Token>.Context context)
    {
        var expr = ExpressionProduction.Parse(context);
        var semiColon = context.Consume(Calculator.Token.SEMICOLON, "Expected ';'.");
        return new ExpressionStatement
        {
            Token = expr.Token,
            Expr = expr,
            SemiColon = semiColon
        };
    }
}