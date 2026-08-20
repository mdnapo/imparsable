using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public partial class ForStatement : ISyntax, IProduction
{
    public Lexer<Token>.Token Token => Keyword;
    public required Lexer<Token>.Token Keyword { get; init; }
    public required Lexer<Token>.Token LeftParenthesis { get; init; }
    public ISyntax? Initializer { get; init; }
    public required ISyntax Condition { get; init; }
    public ISyntax? Increment { get; init; }
    public required Lexer<Token>.Token RightParenthesis { get; init; }
    public required ISyntax Body { get; init; }
    public SymbolTable SymbolTable { get; } = new();

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var keyword = context.Previous();

        var leftParenthesis = context.Consume(Syntax.Token.LEFT_PARENTHESIS, "Expected '(' after for.");

        ISyntax? initializer;
        if (context.Match(Syntax.Token.SEMICOLON))
        {
            initializer = null;
        }
        else if (context.Match(Syntax.Token.VAR))
        {
            initializer = VarStatement.Parse(context);
        }
        else
        {
            initializer = ExpressionStatement.Parse(context);
        }

        var condition = context.Check(Syntax.Token.SEMICOLON)
            ? new BoolLiteralExpression { Token = context.Current, Value = true }
            : Expression.Parse(context);

        context.Consume(Syntax.Token.SEMICOLON, "Expected ';' after for loop condition.");

        var increment = !context.Check(Syntax.Token.RIGHT_PARENTHESIS)
            ? Expression.Parse(context)
            : null;

        var rightParenthesis = context.Consume(Syntax.Token.RIGHT_PARENTHESIS, "Expected ')' after for clauses.");

        if (context.Check(Syntax.Token.CONST))
            context.Diagnostics.Error(context.Peek(), "The body of a while loop cannot be a const statement.");
        if (context.Check(Syntax.Token.VAR))
            context.Diagnostics.Error(context.Peek(), "The body of a while loop cannot be a var statement.");

        var body = Statement.Parse(context);

        return new ForStatement
        {
            Keyword = keyword,
            LeftParenthesis = leftParenthesis,
            Initializer = initializer,
            Condition = condition,
            Increment = increment,
            RightParenthesis = rightParenthesis,
            Body = body
        };
    }
}