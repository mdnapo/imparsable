using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public partial class BinaryExpression : ISyntax, IProduction
{
    private static readonly Token[] AssignmentOperators = [Syntax.Token.EQUALS];
    private static readonly Token[] ConcatenationOperators = [Syntax.Token.DOT];
    private static readonly Token[] AdditionSubtractionOperators = [Syntax.Token.PLUS, Syntax.Token.MINUS];
    private static readonly Token[] MultiplicationDivisionOperators = [Syntax.Token.STAR, Syntax.Token.SLASH];

    public required Lexer<Token>.Token Token { get; init; }
    public required ISyntax LeftOperand { get; init; }
    public required Lexer<Token>.Token Op { get; init; }
    public required ISyntax RightOperand { get; init; }

    public static ISyntax Parse(Parser<Token>.Context context) => Assignment(context);

    private static ISyntax Assignment(Parser<Token>.Context context) =>
        Parse(context, AssignmentOperators, Concatenation);

    private static ISyntax Concatenation(Parser<Token>.Context context) =>
        Parse(context, ConcatenationOperators, AdditionSubtraction);

    private static ISyntax AdditionSubtraction(Parser<Token>.Context context) =>
        Parse(context, AdditionSubtractionOperators, MultiplicationDivision);

    private static ISyntax MultiplicationDivision(Parser<Token>.Context context) =>
        Parse(context, MultiplicationDivisionOperators, UnaryExpression.Parse);

    private static ISyntax Parse(
        Parser<Token>.Context context,
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
                Op = @operator,
                RightOperand = right
            };
        }

        return expr;
    }
}