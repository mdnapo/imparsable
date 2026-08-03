namespace Imparsable.Parsing;

public partial class Lexer<TToken>
{
    public abstract partial class Rule
    {
        public class MultiCharacterRule(TToken type, string @string) : Rule
        {
            public override bool Match(Context ctx)
            {
                var src = ctx.Source;
                int line = src.Line, column = src.Column;
                
                if (!src.Match(@string)) return false;

                var lexeme = src.Extract();
                ctx.Tokens.Add(new Token(src.File, type, lexeme, line, column));

                return true;
            }
        }
    }
}