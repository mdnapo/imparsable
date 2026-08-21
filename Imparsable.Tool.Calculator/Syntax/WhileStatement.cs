using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

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
        var leftParenthesis = context.Consume(Syntax.Token.LEFT_PARENTHESIS, "Expected '(' after while.");
        var condition = Expression.Parse(context);
        var rightParenthesis = context.Consume(Syntax.Token.RIGHT_PARENTHESIS, "Expected ')' after while condition.");

        if (context.Check(Syntax.Token.CONST))
            context.Diagnostics.Error(context.Peek(), "The body of a while loop cannot be a const statement.");
        if (context.Check(Syntax.Token.VAR))
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