using Imparsable.Parsing.Exceptions;

namespace Imparsable.Parsing;

public partial class Lexer<TToken>(
    Lexer<TToken>.Context ctx,
    IEnumerable<Lexer<TToken>.Rule> rules,
    IEnumerable<Lexer<TToken>.Keyword> keywords,
    DiagnosticsCollector diagnostics
) where TToken : Enum
{
    private readonly List<Rule> _rules = [.. rules];

    private Parser<TToken>.Context ParserContext => new(ctx.Configuration, ctx.Tokens);

    public Parser<TToken>.Context Execute()
    {
        if (ctx.Source.Ended()) return ParserContext;

        while (!ctx.Source.Ended())
        {
            try
            {
                if (!_rules.Any(rule => rule.Match(ctx)))
                    ctx.MarkUnexpected();
            }
            catch (SyntaxException e)
            {
                diagnostics.Error(e.Marker, e.Message);
                throw;
            }
        }

        ctx.AddToken(ctx.Configuration.End, "<END>", ctx.Source.Line, ctx.Source.Column);

        var tokens = ApplyKeywords(HandleUnexpectedTokens(ctx.Tokens));
        ctx.Tokens.Clear();
        ctx.Tokens.AddRange(tokens);

        return ParserContext;
    }

    private IEnumerable<Token> HandleUnexpectedTokens(List<Token> tokens)
    {
        foreach (var token in tokens.Where(token => token.Type.Equals(ctx.Configuration.Unexpected)))
            diagnostics.Error(token, $"Unexpected token '{token.Lexeme}'.");

        return tokens.Where(token => !token.Type.Equals(ctx.Configuration.Unexpected));
    }

    private List<Token> ApplyKeywords(IEnumerable<Token> tokens)
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