namespace Imparsable.Parsing;

public interface IProduction<TToken, TSyntax>
    where TToken : Enum
    where TSyntax : ISyntax<TToken>
{
    public static abstract TSyntax Parse(Parser<TToken>.Context context);
}

public interface ISynchronizer<TToken> where TToken : Enum
{
    public static abstract bool Synchronize(Parser<TToken>.Context context);
}

public interface ISyntaxTreeCreator<TToken, TSyntax, out TSyntaxTree>
    where TToken : Enum
    where TSyntax : ISyntax<TToken>
    where TSyntaxTree : SyntaxTree<TToken, TSyntax, TSyntaxTree>, ISyntaxTreeCreator<TToken, TSyntax, TSyntaxTree>
{
    public static abstract TSyntaxTree Create();
}

public abstract class SyntaxTree<TToken, TSyntax, TSyntaxTree> : ISyntaxTreeCreator<TToken, TSyntax, TSyntaxTree>
    where TToken : Enum
    where TSyntax : ISyntax<TToken>
    where TSyntaxTree : SyntaxTree<TToken, TSyntax, TSyntaxTree>, ISyntaxTreeCreator<TToken, TSyntax, TSyntaxTree>
{
    public abstract Parser<TToken>.Configuration Configuration { get; init; }
    public required Source Source { get; init; }
    public DiagnosticsProvider Diagnostics { get; set; } = new();
    public List<TToken> Tokens { get; } = [];
    public List<TSyntax> Roots { get; } = [];
    public SymbolTable SymbolTable { get; set; } = new();

    public static TSyntaxTree Create() => throw new NotImplementedException();
    public Lexer<TToken>.Context BuildLexerContext() => new(Configuration, Source, Diagnostics);


    public static List<TSyntax> Execute<TProduction>() where TProduction : IProduction<TToken, TSyntax>
    {
        var tree = Create();
        var lexerCtx = tree.BuildLexerContext();

        // while (!context.Ended())
        // {
        //     try
        //     {
        //         syntax.Add(TProduction.Parse(context));
        //     }
        //     catch (SyntaxException e)
        //     {
        //         diagnostics.Error(e.Marker, e.Message);
        //         break;
        //     }
        // }
        //
        // return syntax;
        return tree.Roots;
    }

    public static List<TSyntax> Execute<TProduction, TSynchronizer>()
        where TProduction : IProduction<TToken, TSyntax>
        where TSynchronizer : ISynchronizer<TToken>
    {
        // var context = lexer.Execute();
        var syntax = new List<TSyntax>();

        // while (!context.Ended())
        // {
        //     try
        //     {
        //         syntax.Add(TProduction.Parse(context));
        //     }
        //     catch (SyntaxException e)
        //     {
        //         diagnostics.Error(e.Marker, e.Message);
        //         if (!TSynchronizer.Synchronize(context))
        //             break;
        //     }
        // }

        return syntax;
    }
}