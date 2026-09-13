using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class ElseIfStatement : ISyntax, IProduction
{
    public Lexer<Token>.Token Token => ElseKeyword;
    public required Lexer<Token>.Token ElseKeyword { get; init; }
    public required Lexer<Token>.Token IfKeyword { get; init; }
    public required Lexer<Token>.Token LeftParenthesis { get; init; }
    public required ISyntax Condition { get; init; }
    public required Lexer<Token>.Token RightParenthesis { get; init; }
    public required ISyntax Body { get; init; }
    public ISyntax? Next { get; init; }

    public static ISyntax? Parse(ParserContext<Token> context)
    {
        while (!context.Ended() && context.CheckSequence(Parsing.Token.ELSE, Parsing.Token.IF))
        {
            var elseKeyword = context.Advance();
            var ifKeyword = context.Advance();

            var leftParenthesis = context.Consume(Parsing.Token.LEFT_PARENTHESIS, "Expected '(' after else if.");
            var condition = Expression.Parse(context);
            var rightParenthesis = context.Consume(Parsing.Token.RIGHT_PARENTHESIS, "Expected ')' after else if condition.");

            var then = Statement.Parse(context);
            var next = Parse(context);

            return new ElseIfStatement
            {
                ElseKeyword = elseKeyword,
                IfKeyword = ifKeyword,
                LeftParenthesis = leftParenthesis,
                Condition = condition,
                RightParenthesis = rightParenthesis,
                Body = then,
                Next = next
            };
        }

        return null;
    }
}