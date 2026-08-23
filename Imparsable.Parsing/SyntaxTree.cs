using Imparsable.Parsing.Exceptions;
using Imparsable.Parsing.Interfaces;

namespace Imparsable.Parsing;

public abstract class SyntaxTree<TToken, TSyntax, TSyntaxTree> : IDisposable
    where TToken : Enum
    where TSyntax : ISyntax<TToken>
    where TSyntaxTree : SyntaxTree<TToken, TSyntax, TSyntaxTree>, new()
{
    public Source Source { get; init; } = null!;
    public DiagnosticsProvider Diagnostics { get; } = new();
    public List<Lexer<TToken>.Token> Tokens { get; private set; } = [];
    public List<TSyntax> Roots { get; } = [];
    public bool IsHealthy => Diagnostics.All(x => x.Severity != DiagnosticSeverity.ERROR);

    private static TSyntaxTree Create(string source, Action<Diagnostic>? diagnosticPublishedHandler = null)
    {
        var tree = new TSyntaxTree { Source = new Source(source) };

        if (diagnosticPublishedHandler != null)
        {
            tree.Diagnostics.Published += diagnosticPublishedHandler;
        }

        return tree;
    }

    public static TSyntaxTree Parse<TProduction>(string source, Action<Diagnostic>? diagnosticHandler = null)
        where TProduction : IProduction<TToken, TSyntax>
    {
        var tree = Create(source, diagnosticHandler);
        var configuration = ParserConfiguration<TToken>.Default;
        var lexerContext = new Lexer<TToken>.Context(configuration, tree.Diagnostics, tree.Source);

        tree.Tokens = Lexer<TToken>.Default.Execute(lexerContext);

        var parserContext = new ParserContext<TToken>(configuration, tree.Diagnostics, tree.Source, tree.Tokens);

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
                tree.Diagnostics.Error(e.Marker, e.Message);
                break;
            }
        }

        return tree;
    }

    public static TSyntaxTree Parse<TProduction, TSynchronizer>(
        string source,
        Action<Diagnostic>? diagnosticHandler = null
    )
        where TProduction : IProduction<TToken, TSyntax>
        where TSynchronizer : ISynchronizer<TToken>
    {
        var tree = Create(source, diagnosticHandler);
        var configuration = ParserConfiguration<TToken>.Default;
        var lexerContext = new Lexer<TToken>.Context(configuration, tree.Diagnostics, tree.Source);

        tree.Tokens = Lexer<TToken>.Default.Execute(lexerContext);

        var parserContext = new ParserContext<TToken>(configuration, tree.Diagnostics, tree.Source, tree.Tokens);

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
                tree.Diagnostics.Error(e.Marker, e.Message);
                if (!TSynchronizer.Synchronize(parserContext))
                    break;
            }
        }

        return tree;
    }

    public void Dispose()
    {
        Diagnostics.Dispose();
    }
}