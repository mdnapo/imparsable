namespace Imparsable.Parsing;

public partial class Lexer<TToken>
{
    public abstract partial class Rule
    {
        public class SingleCharacterRule(TToken type, char @char) : Rule
        {
            public override bool Match(Context ctx)
            {
                var src = ctx.Source;
                int line = src.Line, column = src.Column;
                
                if (!src.Match(@char)) return false;
                
                var lexeme = src.Extract();
                ctx.AddToken(type, lexeme, line, column);

                return true;
            }
        }
    }
}
