namespace Imparsable.Tools.Parsing.Attributes;

public sealed class WhitespaceAttribute<TToken>(bool ignore = true) : LexerRuleAttribute<TToken> where TToken : Enum
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly char[] WhitespaceOrCarriageReturn = [' ', '\r'];

    public override int Priority => 10;

    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int line = src.Line, column = src.Column;

        if (src.MatchAny(WhitespaceOrCarriageReturn))
        {
            while (src.CheckAny(WhitespaceOrCarriageReturn) && !src.Ended())
                src.Advance();

            HandleIgnore(ignore, Type, context, src, line, column);

            return true;
        }

        if (src.Match('\t'))
        {
            // We do -1 because the call to Match already increments src.Column by 1.
            do src.Column += context.Configuration.TabSize - 1;
            while (src.Match('\t') && !src.Ended());

            HandleIgnore(ignore, Type, context, src, line, column);

            return true;
        }

        return false;
    }
}