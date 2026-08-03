using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public class GroupingExpression : ISyntax, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }
    public required Lexer<Token>.Token LeftParenthesis { get; init; }
    public required ISyntax Expression { get; init; }
    public required Lexer<Token>.Token RightParenthesis { get; init; }

    public static ISyntax Parse(Parser<Token>.Context context)
    {
        var leftParenthesis = context.Previous();
        var expression = ExpressionProduction.Parse(context);
        var rightParenthesis = context.Consume(Syntax.Token.RIGHT_PARENTHESIS, "Expected ')'.");

        return new GroupingExpression
        {
            Token = leftParenthesis,
            LeftParenthesis = leftParenthesis,
            Expression = expression,
            RightParenthesis = rightParenthesis
        };
    }
}