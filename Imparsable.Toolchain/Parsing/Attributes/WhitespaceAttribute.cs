namespace Imparsable.Toolchain.Parsing.Attributes;

public sealed class WhitespaceAttribute<TToken> : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override int Priority => 10;

    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int line = src.Line, column = src.Column;

        if (src.Match(' '))
        {
            while (src.Check(' ') && !src.Ended())
                src.Advance();

            Extract(context, src, line, column);

            return true;
        }

        if (src.Match('\t'))
        {
            // We do -1 because the call to Match already increments src.Column by 1.
            do src.Column += context.Configuration.TabSize - 1;
            while (src.Match('\t') && !src.Ended());

            Extract(context, src, line, column);

            return true;
        }

        return false;
    }

    private void Extract(Lexer<TToken>.Context context, Source src, int line, int column)
    {
        var range = src.Extract();
        context.AddToken(Type, range.Offset, range.Length, line, column);
    }
}