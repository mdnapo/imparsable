using System.Runtime.InteropServices;
using Imparsable.Toolchain.Parsing.Exceptions;
using Imparsable.Toolchain.Parsing.Interfaces;

namespace Imparsable.Toolchain.Parsing;

public abstract class SyntaxTree<TToken, TSyntax, TSyntaxTree>
    where TToken : Enum
    where TSyntax : ISyntax<TToken>
    where TSyntaxTree : SyntaxTree<TToken, TSyntax, TSyntaxTree>, new()
{
    public Source Source { get; init; } = new(string.Empty);
    public List<Lexer<TToken>.Token> Tokens { get; init; } = [];
    public List<Lexer<TToken>.Token> Trivia { get; init; } = [];
    public Dictionary<Lexer<TToken>.Token, Range> TriviaIndex { get; init; } = [];
    public List<TSyntax> Roots { get; } = [];

    public Span<Lexer<TToken>.Token> GetTrivia(Lexer<TToken>.Token token) =>
        CollectionsMarshal.AsSpan(Trivia)[TriviaIndex.GetValueOrDefault(token)];

    public static TSyntaxTree Parse<TProduction>(string text, DiagnosticsProvider diagnostics)
        where TProduction : IProduction<TToken, TSyntax>
    {
        var source = new Source(text);
        var configuration = ParserConfiguration<TToken>.Default;
        var lexerContext = new Lexer<TToken>.Context(configuration, diagnostics, source);

        Lexer<TToken>.Default.Execute(lexerContext);

        var tree = new TSyntaxTree
        {
            Source = source,
            Tokens = lexerContext.Tokens,
            Trivia = lexerContext.Trivia,
            TriviaIndex = lexerContext.TriviaIndex
        };

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
}