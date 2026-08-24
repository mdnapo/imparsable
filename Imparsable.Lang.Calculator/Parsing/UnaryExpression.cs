using Imparsable.Tools.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class UnaryExpression : ISyntax, IProduction
{
    private static readonly Token[] Operators = [Parsing.Token.BANG, Parsing.Token.MINUS];
    public required Lexer<Token>.Token Token { get; init; }
    public required ISyntax Operand { get; init; }
    public required Lexer<Token>.Token Op { get; init; }

    public static ISyntax Parse(ParserContext<Token> context)
    {
        if (context.MatchAny(Operators))
        {
            var op = context.Previous();
            var operand = Parse(context);

            return new UnaryExpression
            {
                Token = op,
                Operand = operand,
                Op = op
            };
        }

        return PrimaryExpression.Parse(context);
    }
}