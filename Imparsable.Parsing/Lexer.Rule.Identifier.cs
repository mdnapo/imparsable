namespace Imparsable.Parsing;

public partial class Lexer<TToken>
{
    public abstract partial class Rule
    {
        public class Identifier(TToken type) : Rule
        {
            public override bool Match(Context context)
            {
                var src = context.Source;

                if (!IsAlpha(src.Peek())) return false;

                int line = src.Line, column = src.Column;

                while (IsAlphaNumeric(src.Peek())) src.Advance();

                var lexeme = src.Extract();

                context.AddToken(type, lexeme, line, column);

                return true;
            }
        }
    }
}