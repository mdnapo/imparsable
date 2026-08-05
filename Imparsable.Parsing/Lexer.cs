using Imparsable.Parsing.Exceptions;

namespace Imparsable.Parsing;

public partial class Lexer<TToken>(
    Lexer<TToken>.Context ctx,
    IEnumerable<Lexer<TToken>.Rule> rules,
    DiagnosticsProvider diagnostics
) where TToken : Enum
{
    private Parser<TToken>.Context ParserContext => new(ctx.Configuration, ctx.Tokens);

    public Parser<TToken>.Context Execute()
    {
        if (ctx.Source.Ended()) return ParserContext;

        while (!ctx.Source.Ended())
        {
            try
            {
                if (!rules.Any(rule => rule.Match(ctx)))
                    ctx.MarkUnexpected();
            }
            catch (SyntaxException e)
            {
                diagnostics.Error(e.Marker, e.Message);
                ctx.Tokens.Clear();
                break;
            }
        }
        
        ctx.Complete();

        return ParserContext;
    }
}