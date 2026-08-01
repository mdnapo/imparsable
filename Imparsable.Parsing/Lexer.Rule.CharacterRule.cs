namespace Imparsable.Parsing;

public abstract partial class Lexer<TToken>
{
    public abstract partial class Rule
    {
        public class CharacterRule(TToken type, char @char) : Rule
        {
            public override bool Match(Context ctx)
            {
                if (!ctx.Source.Match(@char)) return false;

                var line = ctx.Source.Line;
                var column = ctx.Source.Column;
                var lexeme = ctx.Source.Extract();

                ctx.Source.Column += 1;

                ctx.AddToken(type, lexeme, line, column);

                return true;
            }
        }
    }
}
