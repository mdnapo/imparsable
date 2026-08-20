using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public partial class AssignmentExpression : ISyntax, IProduction
{
    private static readonly Token[] Operators =
    [
        Syntax.Token.EQUAL,
        Syntax.Token.PLUS_EQUAL,
        Syntax.Token.MINUS_EQUAL,
        Syntax.Token.STAR_EQUAL,
        Syntax.Token.SLASH_EQUAL,
    ];

    public Lexer<Token>.Token Token => Operator;
    public required ISyntax Target { get; init; }
    public required Lexer<Token>.Token Operator { get; init; }
    public required ISyntax Value { get; init; }

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var expr = BinaryExpression.Parse(context);

        if (context.MatchAny(Operators))
        {
            var @operator = context.Previous();
            var value = Parse(context);

            return new AssignmentExpression
            {
                Target = expr,
                Operator = @operator,
                Value = value
            };
        }

        return expr;
    }
}