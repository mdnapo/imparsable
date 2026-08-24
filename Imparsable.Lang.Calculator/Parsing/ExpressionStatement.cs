using Imparsable.Tools.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class ExpressionStatement : ISyntax, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }
    public required ISyntax Expression { get; init; }
    public required Lexer<Token>.Token SemiColon { get; set; }

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var expr = Parsing.Expression.Parse(context);
        var semiColon = context.Consume(Parsing.Token.SEMICOLON, "Expected ';'.");
        
        return new ExpressionStatement
        {
            Token = expr.Token,
            Expression = expr,
            SemiColon = semiColon
        };
    }
}