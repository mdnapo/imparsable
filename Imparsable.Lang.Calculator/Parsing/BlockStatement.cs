using System.Collections;
using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;
using Imparsable.Toolchain.Parsing.Interfaces;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class BlockStatement : ISyntax, ISymbolTable, IProduction
{
    public Lexer<Token>.Token Token => LeftBrace;
    public required ISymbolTable Symbols { get; init; }
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
            Symbols = new SymbolTable(context.Source),
            LeftBrace = leftBrace,
            Body = stmts,
            RightBrace = rightBrace,
        };
    }

    public IEnumerator<ISymbol> GetEnumerator() => Symbols.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Symbols).GetEnumerator();

    public ISymbolTable? Parent
    {
        get => Symbols.Parent;
        set => Symbols.Parent = value;
    }

    public int StackDepth => Symbols.StackDepth;

    public void Add(ISymbol symbol) => Symbols.Add(symbol);

    public ISymbol? Lookup(ISourceMarker symbol) => Symbols.Lookup(symbol);

    public ISymbol? RecursiveLookup(ISourceMarker symbol) => Symbols.RecursiveLookup(symbol);

    public int Offset(ISourceMarker symbol) => Symbols.Offset(symbol);
}