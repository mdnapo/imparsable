namespace Imparsable.Parsing;

public partial class Lexer<TToken>(
    Lexer<TToken>.Context ctx,
    IEnumerable<Lexer<TToken>.Rule> rules,
    IEnumerable<Lexer<TToken>.Keyword> keywords
) where TToken : Enum
{
    private readonly List<Rule> _rules = [.. rules];

    public List<Token> Execute()
    {
        while (!ctx.Source.Ended())
            if (!_rules.Any(rule => rule.Match(ctx)))
                ctx.MarkUnexpected();

        ctx.AddToken(ctx.Configuration.End, "<END>", ctx.Source.Line, ctx.Source.Column);

        var tokens = ApplyKeywords(ctx.Tokens);
        ctx.Tokens.Clear();
        ctx.Tokens.AddRange(tokens);

        return tokens;
    }

    private List<Token> ApplyKeywords(List<Token> tokens)
    {
        return
        [
            .. tokens.Select(token =>
                token.Type.Equals(ctx.Configuration.Identifier) &&
                keywords.FirstOrDefault(k => k.Name == token.Lexeme) is { } keyword
                    ? new Token(token.File, keyword.Type, token.Lexeme, token.Line, token.Column)
                    : token
            )
        ];
    }
}