namespace Imparsable.Parsing;

public abstract partial class Lexer<TToken>
{
    public abstract partial class Rule
    {
        public class NewLine : Rule
        {
            public override bool Match(Context context)
            {
                if (!context.Source.Match('\n')) return false;

                context.Source.Line++;
                context.Source.Column = 1;
                context.Source.Ignore();

                return true;
            }
        }
    }
}