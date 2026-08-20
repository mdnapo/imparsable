using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public partial class ExpressionStatement : ISyntax, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }
    public required ISyntax Expression { get; init; }
    public required Lexer<Token>.Token SemiColon { get; set; }

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var expr = Syntax.Expression.Parse(context);
        var semiColon = context.Consume(Syntax.Token.SEMICOLON, "Expected ';'.");
        
        return new ExpressionStatement
        {
            Token = expr.Token,
            Expression = expr,
            SemiColon = semiColon
        };
    }
}