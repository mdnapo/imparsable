using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public partial class ElseIfStatement : ISyntax, IProduction
{
    public Lexer<Token>.Token Token => ElseKeyword;
    public required Lexer<Token>.Token ElseKeyword { get; init; }
    public required Lexer<Token>.Token IfKeyword { get; init; }
    public required ISyntax Condition { get; init; }
    public required ISyntax Body { get; init; }
    public ISyntax? Next { get; init; }

    public static ISyntax? Parse(ParserContext<Token> context)
    {
        while (!context.Ended() && context.CheckSequence(Syntax.Token.ELSE, Syntax.Token.IF))
        {
            var elseKeyword = context.Advance();
            var ifKeyword = context.Advance();

            context.Consume(Syntax.Token.LEFT_PARENTHESIS, "Expected '(' after else if.");
            var condition = Expression.Parse(context);
            context.Consume(Syntax.Token.RIGHT_PARENTHESIS, "Expected ')' after else if condition.");

            var then = Statement.Parse(context);
            var next = Parse(context);

            return new ElseIfStatement
            {
                ElseKeyword = elseKeyword,
                IfKeyword = ifKeyword,
                Condition = condition,
                Body = then,
                Next = next
            };
        }

        return null;
    }
}