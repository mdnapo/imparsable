using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class GroupingExpression : ISyntax, IProduction
{
    public required Lexer<Token>.Token Token { get; init; }
    public required Lexer<Token>.Token LeftParenthesis { get; init; }
    public required ISyntax Expression { get; init; }
    public required Lexer<Token>.Token RightParenthesis { get; init; }

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var leftParenthesis = context.Previous();
        var expression = Parsing.Expression.Parse(context);
        var rightParenthesis = context.Consume(Parsing.Token.RIGHT_PARENTHESIS, "Expected ')'.");

        return new GroupingExpression
        {
            Token = leftParenthesis,
            LeftParenthesis = leftParenthesis,
            Expression = expression,
            RightParenthesis = rightParenthesis
        };
    }
}