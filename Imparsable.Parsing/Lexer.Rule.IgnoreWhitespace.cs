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

                if (src.Match('\r'))
                {
                    src.Ignore();
                    return true;
                }

                if (src.Match(' '))
                {
                    src.Ignore();
                    src.Column += configuration.SpaceSize;
                    return true;
                }

                if (src.Match('\t'))
                {
                    src.Ignore();
                    src.Column += configuration.TabSize;
                    return true;
                }

                return false;
            }
        }
    }
}