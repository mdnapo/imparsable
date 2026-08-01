namespace Imparsable.Parsing;

public abstract partial class Lexer<TToken>(
    Parser<TToken>.Configuration configuration,
    IEnumerable<Lexer<TToken>.Rule> rules,
    IEnumerable<Lexer<TToken>.Keyword> keywords
) where TToken : Enum
{
    private readonly List<Rule> _rules = [.. rules];

    protected abstract TToken End { get; }

    public List<Token> Lex(string file, string source)
    {
        var ctx = new Context(configuration, file, new Source(file, source), []);

        while (!ctx.Source.Ended())
            if (!_rules.Any(rule => rule.Match(ctx)))
                ctx.MarkUnexpected();

        ctx.AddToken(End, "<END>", ctx.Source.Line, ctx.Source.Column);

        return ApplyKeywords(ctx.Tokens);
    }

    private List<Token> ApplyKeywords(List<Token> tokens) =>
    [
        .. tokens.Select(token => keywords.FirstOrDefault(k => k.Name == token.Lexeme) is { } keyword
            ? new Token(token.File, keyword.Type, token.Lexeme, token.Line, token.Column)
            : token
        )
    ];

    // protected static bool Match(Context ctx, TToken type, char @char)
    // {
    //     if (!ctx.Source.Match(@char)) return false;
    //
    //     var line = ctx.Source.Line;
    //     var column = ctx.Source.Column;
    //     var lexeme = ctx.Source.Extract();
    //
    //     ctx.Source.Column += 1;
    //
    //     ctx.AddToken(type, lexeme, line, column);
    //
    //     return true;
    // }

    /*
     * Do not use this method for keywords!
     * This will treat an identifier named 'ifelse' as separated 'if' and 'else' tokens.
     */
    // protected static bool Match(Context ctx, TToken token, string sequence)
    // {
    //     if (!ctx.Source.Match(sequence)) return false;
    //
    //     var line = ctx.Source.Line;
    //     var column = ctx.Source.Column;
    //     var lexeme = ctx.Source.Extract();
    //     ctx.Source.Column += sequence.Length;
    //     ctx.Tokens.Add(new Token(ctx.File, token, lexeme, line, column));
    //
    //     return true;
    // }
}