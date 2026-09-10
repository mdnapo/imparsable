namespace Imparsable.Toolchain.Parsing.Attributes;

public sealed class SingleLineCommentAttribute<TToken>(bool ignore = true) : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override int Priority => 20;

    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int line = src.Line, column = src.Column;

        if (!src.MatchSequence("//")) return false;

        while (!src.CheckSequence(Environment.NewLine) && !src.Ended())
            src.Advance();

        HandleIgnore(ignore, Type, context, src, line, column);

        context.Source.Column = 1;

        return true;
    }
}

public sealed class MultiLineCommentAttribute<TToken>(bool ignore = true) : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override int Priority => 20;

    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int line = src.Line, column = src.Column;

        if (!src.MatchSequence("/*")) return false;

        var lines = 1;
        while (!src.CheckSequence("*/") && !src.Ended())
        {
            var previous = src.Advance();
            lines += previous == '\n' ? 1 : 0;
        }

        if (src.Ended())
        {
            context.Halt("Unterminated comment.");
        }

        src.MatchSequence("*/");

        HandleIgnore(ignore, Type, context, src, line, column);

        context.Source.Line += lines;
        context.Source.Column = 1;

        return true;
    }
}

public sealed class NewLineAttribute<TToken>(bool ignore = true) : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override int Priority => 20;

    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int line = src.Line, column = src.Column;

        if (!src.MatchSequence(Environment.NewLine)) return false;

        var lines = 1;
        while (src.CheckSequence(Environment.NewLine) && !src.Ended())
        {
            foreach (var _ in Environment.NewLine)
                src.Advance();

            lines++;
        }

        HandleIgnore(ignore, Type, context, src, line, column);

        context.Source.Line += lines;
        context.Source.Column = 1;

        return true;
    }
}