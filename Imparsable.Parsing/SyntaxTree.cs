using Imparsable.Parsing.Exceptions;
using Imparsable.Parsing.Interfaces;

namespace Imparsable.Parsing;

public abstract class SyntaxTree<TToken, TSyntax, TSyntaxTree> : ISyntaxTree<TToken, TSyntax, TSyntaxTree>
    where TToken : Enum
    where TSyntax : ISyntax<TToken>
    where TSyntaxTree : SyntaxTree<TToken, TSyntax, TSyntaxTree>, ISyntaxTree<TToken, TSyntax, TSyntaxTree>, new()
{
    public Source Source { get; init; } = null!;
    public DiagnosticsProvider Diagnostics { get; } = new();
    public List<Lexer<TToken>.Token> Tokens { get; private set; } = [];
    public List<TSyntax> Roots { get; } = [];
    public SymbolTable SymbolTable { get; } = new();

    public static TSyntaxTree Create(string source) => new() { Source = new Source(source) };

    public static TSyntaxTree Parse<TProduction>(string source)
        where TProduction : IProduction<TToken, TSyntax>
    {
        var tree = Create(source);
        var configuration = ParserConfiguration<TToken>.Default;
        var lexerContext = new Lexer<TToken>.Context(configuration, tree.Source, tree.Diagnostics);

        tree.Tokens = Lexer<TToken>.Default.Execute(lexerContext);

        var parserContext = new ParserContext<TToken>(configuration, tree.Source, tree.Tokens);

        while (!parserContext.Ended())
        {
            try
            {
                tree.Roots.Add(TProduction.Parse(parserContext));
            }
            catch (SyntaxException e)
            {
                tree.Diagnostics.Error(e.Marker, e.Message);
                break;
            }
        }

        return tree;
    }

    public static TSyntaxTree Parse<TProduction, TSynchronizer>(string source)
        where TProduction : IProduction<TToken, TSyntax>
        where TSynchronizer : ISynchronizer<TToken>
    {
        var tree = Create(source);
        var configuration = ParserConfiguration<TToken>.Default;
        var lexerContext = new Lexer<TToken>.Context(configuration, tree.Source, tree.Diagnostics);

        tree.Tokens = Lexer<TToken>.Default.Execute(lexerContext);

        var parserContext = new ParserContext<TToken>(configuration, tree.Source, tree.Tokens);

        while (!parserContext.Ended())
        {
            try
            {
                tree.Roots.Add(TProduction.Parse(parserContext));
            }
            catch (SyntaxException e)
            {
                tree.Diagnostics.Error(e.Marker, e.Message);
                if (!TSynchronizer.Synchronize(parserContext))
                    break;
            }
        }

        return tree;
    }
}