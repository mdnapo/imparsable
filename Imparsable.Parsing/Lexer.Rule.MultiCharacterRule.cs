namespace Imparsable.Parsing;

public abstract partial class Lexer<TToken>
{
    public abstract partial class Rule
    {
        public class MultiCharacterRule(TToken type, string @string) : Rule
        {
            public override bool Match(Context ctx)
            {
                if (!ctx.Source.Match(@string)) return false;

                var line = ctx.Source.Line;
                var column = ctx.Source.Column;
                var lexeme = ctx.Source.Extract();
                ctx.Source.Column += @string.Length;
                ctx.Tokens.Add(new Token(ctx.File, type, lexeme, line, column));

                return true;
            }
        }
    }
}