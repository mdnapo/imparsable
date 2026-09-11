namespace Imparsable.Toolchain.Parsing.Attributes;

public sealed class SingleLineCommentAttribute<TToken> : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override int Priority => 20;

    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int line = src.Line, column = src.Column;

        if (!src.MatchSequence("//")) return false;

        while (!src.CheckSequence(Environment.NewLine) && !src.Ended())
            src.Advance();

        var range = src.Extract();
        context.AddToken(Type, range.Offset, range.Length, line, column);
        context.Source.Column = 1;

        return true;
    }
}