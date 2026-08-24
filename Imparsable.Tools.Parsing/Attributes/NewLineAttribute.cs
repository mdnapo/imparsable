namespace Imparsable.Tools.Parsing.Attributes;

public sealed class NewLineAttribute<TToken>(bool ignore = true) : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override int Priority => 20;

    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int line = src.Line, column = src.Column;

        if (!src.Match('\n')) return false;

        var lines = 1;
        while (src.Check('\n') && !src.Ended())
        {
            src.Advance();
            lines++;
        }

        HandleIgnore(ignore, Type, context, src, line, column);

        context.Source.Line += lines;
        context.Source.Column = 1;

        return true;
    }
}