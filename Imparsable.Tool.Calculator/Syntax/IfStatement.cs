using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public partial class IfStatement : ISyntax, IProduction
{
    public Lexer<Token>.Token Token => Keyword;
    public required Lexer<Token>.Token Keyword { get; init; }
    public required Lexer<Token>.Token OpenParentheses { get; init; }
    public required Lexer<Token>.Token CloseParentheses { get; init; }
    public required ISyntax Condition { get; init; }
    public required ISyntax Body { get; init; }
    public ISyntax? ElseIf { get; init; }
    public Lexer<Token>.Token? ElseKeyword { get; init; }
    public ISyntax? Else { get; init; }

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var keyword = context.Previous();
        var leftParenthesis = context.Consume(Syntax.Token.LEFT_PARENTHESIS, "Expected '('.");
        var condition = Expression.Parse(context);
        var rightParenthesis = context.Consume(Syntax.Token.RIGHT_PARENTHESIS, "Expected ')'.");
        var body = Statement.Parse(context);
        var elseIf = context.CheckSequence(Syntax.Token.ELSE, Syntax.Token.IF)
            ? ElseIfStatement.Parse(context)
            : null;
        Lexer<Token>.Token? elseKeyword = context.Match(Syntax.Token.ELSE) ? context.Previous() : null;
        var @else = elseKeyword is null ? null : Statement.Parse(context);

        return new IfStatement
        {
            Keyword = keyword,
            OpenParentheses = leftParenthesis,
            Condition = condition,
            CloseParentheses = rightParenthesis,
            Body = body,
            ElseIf = elseIf,
            ElseKeyword = elseKeyword,
            Else = @else
        };
    }
}