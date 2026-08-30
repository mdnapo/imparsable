using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class BlockStatement : SymbolTable, ISyntax, IProduction
{
    public Lexer<Token>.Token Token => LeftBrace;
    public required Lexer<Token>.Token LeftBrace { get; init; }
    public required List<ISyntax> Body { get; init; }
    public required Lexer<Token>.Token RightBrace { get; init; }

    public static ISyntax Parse(ParserContext<Token> context)
    {
        var leftBrace = context.Previous();

        var stmts = new List<ISyntax>();

        while (!context.Check(Parsing.Token.RIGHT_BRACE) && !context.Ended())
            stmts.Add(Statement.Parse(context));

        var rightBrace = context.Consume(Parsing.Token.RIGHT_BRACE, "Expected '}' after block.");

        return new BlockStatement
        {
            LeftBrace = leftBrace,
            Body = stmts,
            RightBrace = rightBrace,
        };
    }
}