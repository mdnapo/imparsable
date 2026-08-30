using Imparsable.Tools.Parsing.Exceptions;
using Imparsable.Tools.Parsing.Interfaces;

namespace Imparsable.Tools.Parsing;

public abstract class SyntaxTree<TToken, TSyntax, TSyntaxTree>
    where TToken : Enum
    where TSyntax : ISyntax<TToken>
    where TSyntaxTree : SyntaxTree<TToken, TSyntax, TSyntaxTree>, new()
{
    public Source Source { get; init; } = null!;
    public List<Lexer<TToken>.Token> Tokens { get; private set; } = [];
    public List<TSyntax> Roots { get; } = [];

    private static TSyntaxTree Create(string source) => new() { Source = new Source(source) };

    public static TSyntaxTree Parse<TProduction>(string source, DiagnosticsProvider diagnostics)
        where TProduction : IProduction<TToken, TSyntax>
    {
        var tree = Create(source);
        var configuration = ParserConfiguration<TToken>.Default;
        var lexerContext = new Lexer<TToken>.Context(configuration, diagnostics, tree.Source);

        tree.Tokens = Lexer<TToken>.Default.Execute(lexerContext);

        var parserContext = new ParserContext<TToken>(configuration, diagnostics, tree.Source, tree.Tokens);

        while (!parserContext.Ended())
        {
            try
            {
                tree.Roots.Add(
                    TProduction.Parse(parserContext) ??
                    throw new Exception($"Production {typeof(TProduction).Name} produced a null value.")
                );
            }
            catch (SyntaxException e)
            {
                diagnostics.Error(e.Marker, e.Message);
                break;
            }
        }

        return tree;
    }

    public static TSyntaxTree Parse<TProduction, TSynchronizer>(string source, DiagnosticsProvider diagnostics)
        where TProduction : IProduction<TToken, TSyntax>
        where TSynchronizer : ISynchronizer<TToken>
    {
        var tree = Create(source);
        var configuration = ParserConfiguration<TToken>.Default;
        var lexerContext = new Lexer<TToken>.Context(configuration, diagnostics, tree.Source);

        tree.Tokens = Lexer<TToken>.Default.Execute(lexerContext);

        var parserContext = new ParserContext<TToken>(configuration, diagnostics, tree.Source, tree.Tokens);

        while (!parserContext.Ended())
        {
            try
            {
                tree.Roots.Add(
                    TProduction.Parse(parserContext) ??
                    throw new Exception($"Production {typeof(TProduction).Name} produced a null value.")
                );
            }
            catch (SyntaxException e)
            {
                diagnostics.Error(e.Marker, e.Message);
                if (!TSynchronizer.Synchronize(parserContext))
                    break;
            }
        }

        return tree;
    }
}