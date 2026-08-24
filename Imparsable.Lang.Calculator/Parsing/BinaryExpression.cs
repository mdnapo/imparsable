using Imparsable.Tools.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class BinaryExpression : ISyntax, IProduction
{
    private static readonly Token[] EqualityOperators =
    [
        Parsing.Token.BANG_EQUAL,
        Parsing.Token.EQUAL_EQUAL,
        Parsing.Token.LOWER_THAN,
        Parsing.Token.LOWER_EQUAL,
        Parsing.Token.GREATER_THAN,
        Parsing.Token.GREATER_EQUAL,
    ];

    private static readonly Token[] ConcatenationOperators = [Parsing.Token.DOT];
    private static readonly Token[] AdditionSubtractionOperators = [Parsing.Token.PLUS, Parsing.Token.MINUS];
    private static readonly Token[] MultiplicationDivisionOperators = [Parsing.Token.STAR, Parsing.Token.SLASH];

    public required Lexer<Token>.Token Token { get; init; }
    public required ISyntax LeftOperand { get; init; }
    public required Lexer<Token>.Token Operator { get; init; }
    public required ISyntax RightOperand { get; init; }

    public static ISyntax Parse(ParserContext<Token> context) => Equality(context);

    private static ISyntax Equality(ParserContext<Token> context) =>
        Parse(context, EqualityOperators, Concatenation);

    private static ISyntax Concatenation(ParserContext<Token> context) =>
        Parse(context, ConcatenationOperators, AdditionSubtraction);

    private static ISyntax AdditionSubtraction(ParserContext<Token> context) =>
        Parse(context, AdditionSubtractionOperators, MultiplicationDivision);

    private static ISyntax MultiplicationDivision(ParserContext<Token> context) =>
        Parse(context, MultiplicationDivisionOperators, UnaryExpression.Parse);

    private static ISyntax Parse(
        ParserContext<Token> context,
        Token[] operators,
        ISyntax.Func<ISyntax> parser
    )
    {
        var expr = parser(context);

        while (context.MatchAny(operators))
        {
            var @operator = context.Previous();
            var right = parser(context);

            expr = new BinaryExpression
            {
                Token = @operator,
                LeftOperand = expr,
                Operator = @operator,
                RightOperand = right
            };
        }

        return expr;
    }
}