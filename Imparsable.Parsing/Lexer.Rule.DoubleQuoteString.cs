using Imparsable.Parsing.Exceptions;

namespace Imparsable.Parsing;

public partial class Lexer<TToken>
{
    public abstract partial class Rule
    {
        public class DoubleQuoteString(TToken type) : Rule
        {
            public override bool Match(Context context)
            {
                if (!context.Source.Match('"')) return false;

                var src = context.Source;
                int line = src.Line, column = src.Column;

                while (src.Peek() != '"' && !src.Ended())
                {
                    // Account for source lines and columns when dealing with a string spanning multiple lines.
                    if (src.Peek() == '\n')
                    {
                        src.Line++;
                        src.Column = 1;
                    }

                    src.Advance();
                }

                if (src.Ended())
                {
                    SyntaxException.Throw(src, $"Unterminated string near '{src.Last}'.");
                }

                // Include the closing quotation mark.
                src.Advance();

                var lexeme = src.Extract();

                context.AddToken(type, lexeme.Trim('"'), line, column);

                return true;
            }
        }
    }
}