using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class AssignmentExpression : ISyntax, IProduction
{
    private static readonly Token[] Operators =
    [
        Parsing.Token.EQUAL,
        Parsing.Token.PLUS_EQUAL,
        Parsing.Token.MINUS_EQUAL,
        Parsing.Token.STAR_EQUAL,
        Parsing.Token.SLASH_EQUAL,
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