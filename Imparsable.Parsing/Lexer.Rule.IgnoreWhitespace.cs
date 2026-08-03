namespace Imparsable.Parsing;

public partial class Lexer<TToken>
{
    public abstract partial class Rule
    {
        public class IgnoreWhitespace(Parser<TToken>.Configuration configuration) : Rule
        {
            public override bool Match(Context context)
            {
                var src = context.Source;

                if (src.Match('\r') || src.Match(' '))
                {
                    src.Ignore();
                    return true;
                }

                if (src.Match('\t'))
                {
                    src.Ignore();
                    src.Column += configuration.TabSize - 1;
                    return true;
                }

                return false;
            }
        }
    }
}