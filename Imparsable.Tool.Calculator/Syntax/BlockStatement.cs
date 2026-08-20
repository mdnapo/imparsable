using Imparsable.Parsing;

namespace Imparsable.Tool.Calculator.Syntax;

public partial class BlockStatement : ISyntax, IProduction
{
    public Lexer<Token>.Token Token => LeftBrace;
    public required Lexer<Token>.Token LeftBrace { get; init; }
    public required List<ISyntax> Body { get; init; }
    public required Lexer<Token>.Token RightBrace { get; init; }
    public SymbolTable SymbolTable { get; } = new();

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var leftBrace = context.Previous();

        var stmts = new List<ISyntax>();

        while (!context.Check(Syntax.Token.RIGHT_BRACE) && !context.Ended())
            stmts.Add(Statement.Parse(context));

        var rightBrace = context.Consume(Syntax.Token.RIGHT_BRACE, "Expected '}' after block.");

        return new BlockStatement
        {
            LeftBrace = leftBrace,
            Body = stmts,
            RightBrace = rightBrace,
        };
    }
}