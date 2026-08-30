using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class WhileStatement : ISyntax, IProduction
{
    public Lexer<Token>.Token Token => Keyword;
    public Lexer<Token>.Token Keyword { get; init; }
    public Lexer<Token>.Token LeftParenthesis { get; init; }
    public required ISyntax Condition { get; init; }
    public Lexer<Token>.Token RightParenthesis { get; init; }
    public required ISyntax Body { get; init; }

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var keyword = context.Previous();
        var leftParenthesis = context.Consume(Parsing.Token.LEFT_PARENTHESIS, "Expected '(' after while.");
        var condition = Expression.Parse(context);
        var rightParenthesis = context.Consume(Parsing.Token.RIGHT_PARENTHESIS, "Expected ')' after while condition.");

        if (context.Check(Parsing.Token.CONST))
            context.Diagnostics.Error(context.Peek(), "The body of a while loop cannot be a const statement.");
        if (context.Check(Parsing.Token.VAR))
            context.Diagnostics.Error(context.Peek(), "The body of a while loop cannot be a var statement.");

        var body = Statement.Parse(context);

        return new WhileStatement
        {
            Keyword = keyword,
            LeftParenthesis = leftParenthesis,
            Condition = condition,
            RightParenthesis = rightParenthesis,
            Body = body
        };
    }
}