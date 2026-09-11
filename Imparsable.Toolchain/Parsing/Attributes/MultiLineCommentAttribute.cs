namespace Imparsable.Toolchain.Parsing.Attributes;

public sealed class MultiLineCommentAttribute<TToken> : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override int Priority => 20;

    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int offset = src.Offset, line = src.Line, column = src.Column;

        if (!src.MatchSequence("/*")) return false;

        var lines = 1;
        while (!src.CheckSequence("*/") && !src.Ended())
        {
            var previous = src.Advance();
            lines += previous == '\n' ? 1 : 0;
        }

        if (src.Ended())
        {
            var marker = new SourceMarker(offset, src.Length, line, column);
            context.Halt(marker, "Unterminated comment.");
        }

        src.MatchSequence("*/");

        var range = src.Extract();
        context.AddToken(Type, range.Offset, range.Length, line, column);
        context.Source.Line += lines;
        context.Source.Column = 1;

        return true;
    }
}