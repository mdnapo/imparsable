namespace Imparsable.Parsing.Attributes;

public sealed class NewLineAttribute<TToken>(bool ignore = true) : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int line = src.Line, column = src.Column;

        if (!src.Match('\n')) return false;

        while (src.Check('\n') && !src.Ended())
            src.Advance();

        HandleIgnore(ignore, Type, context, src, line, column);

        context.Source.Line++;
        context.Source.Column = 1;

        return true;
    }
}