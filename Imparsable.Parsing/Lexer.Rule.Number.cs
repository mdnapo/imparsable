namespace Imparsable.Parsing;

public abstract partial class Lexer<TToken>
{
    public abstract partial class Rule
    {
        public class Number(TToken type) : Rule
        {
            public override bool Match(Context context)
            {
                var src = context.Source;

                if (!IsDigit(src.Peek())) return false;

                int line = src.Line, column = src.Column;

                ProcessDigits(src);

                if (IsDigit(src.Peek(1)) && src.Match('.'))
                {
                    ProcessDigits(src);
                }

                var lexeme = src.Extract();

                context.AddToken(type, lexeme, line, column);

                return true;
            }
        }
    }
}