using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

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

        var leftParenthesis = context.Consume(Parsing.Token.LEFT_PARENTHESIS, "Expected '(' after for.");

        ISyntax? initializer;
        if (context.Match(Parsing.Token.SEMICOLON))
        {
            initializer = null;
        }
        else if (context.Match(Parsing.Token.VAR))
        {
            initializer = VarStatement.Parse(context);
        }
        else
        {
            initializer = ExpressionStatement.Parse(context);
        }

        var condition = context.Check(Parsing.Token.SEMICOLON)
            ? new BoolLiteralExpression { Token = context.Current, Value = true }
            : Expression.Parse(context);

        context.Consume(Parsing.Token.SEMICOLON, "Expected ';' after for loop condition.");

        var increment = !context.Check(Parsing.Token.RIGHT_PARENTHESIS)
            ? Expression.Parse(context)
            : null;

        var rightParenthesis = context.Consume(Parsing.Token.RIGHT_PARENTHESIS, "Expected ')' after for clauses.");

        if (context.Check(Parsing.Token.CONST))
            context.Diagnostics.Error(context.Peek(), "The body of a while loop cannot be a const statement.");
        if (context.Check(Parsing.Token.VAR))
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