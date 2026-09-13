using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class IfStatement : ISyntax, IProduction
{
    public Lexer<Token>.Token Token => Keyword;
    public required Lexer<Token>.Token Keyword { get; init; }
    public required Lexer<Token>.Token LeftParenthesis { get; init; }
    public required Lexer<Token>.Token RightParenthesis { get; init; }
    public required ISyntax Condition { get; init; }
    public required ISyntax Body { get; init; }
    public ISyntax? ElseIf { get; init; }
    public Lexer<Token>.Token? ElseKeyword { get; init; }
    public ISyntax? Else { get; init; }

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var keyword = context.Previous();
        var leftParenthesis = context.Consume(Parsing.Token.LEFT_PARENTHESIS, "Expected '('.");
        var condition = Expression.Parse(context);
        var rightParenthesis = context.Consume(Parsing.Token.RIGHT_PARENTHESIS, "Expected ')'.");
        var body = Statement.Parse(context);
        var elseIf = context.CheckSequence(Parsing.Token.ELSE, Parsing.Token.IF)
            ? ElseIfStatement.Parse(context)
            : null;
        Lexer<Token>.Token? elseKeyword = context.Match(Parsing.Token.ELSE) ? context.Previous() : null;
        var @else = elseKeyword is null ? null : Statement.Parse(context);

        return new IfStatement
        {
            Keyword = keyword,
            LeftParenthesis = leftParenthesis,
            Condition = condition,
            RightParenthesis = rightParenthesis,
            Body = body,
            ElseIf = elseIf,
            ElseKeyword = elseKeyword,
            Else = @else
        };
    }
}