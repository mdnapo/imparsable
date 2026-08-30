using Imparsable.Toolchain.Parsing.Interfaces;

namespace Imparsable.Toolchain.Parsing;

public class Source(string source) : Stream<char>(source.AsMemory()), ISourceMarker
{
    public record struct Range(int Offset, int Length);

    public int Offset => Start;
    public int Length => Position - Start;

    public int Line { get; set; } = 1;
    public int Column { get; set; } = 1;
    public char Last => Sequence[^1];
    private int Start { get; set; }

    public override char Advance()
    {
        Column++;
        return base.Advance();
    }

    public override bool Ended() => Current == '\0' || base.Ended();

    public Range Extract()
    {
        var range = new Range(Offset, Length);
        Start = Position;
        return range;
    }

    public string GetText(int offset, int length) => source.Substring(offset, length);

    public void Ignore()
    {
        Start = Position;
    }

    public bool Check(char expected) => Peek() == expected;

    public bool CheckAny(char[] expected)
    {
        for (var index = 0; index < expected.Length; index++)
            if (Peek() == expected[index])
                return true;

        return false;
    }

    public bool Match(char expected)
    {
        if (Sequence[Position] != expected) return false;
        Position += 1;
        Column += 1;
        return true;
    }

    public bool MatchAny(char[] expected)
    {
        for (var index = 0; index < expected.Length; index++)
            if (Match(expected[index]))
                return true;

        return false;
    }

    public bool MatchSequence(ReadOnlySpan<char> expected)
    {
        if (Position + expected.Length > Sequence.Length) return false;

        for (var index = 0; index < expected.Length; index++)
        {
            if (Sequence[Position + index] != expected[index])
            {
                return false;
            }
        }

        Position += expected.Length;
        Column += expected.Length;

        return true;
    }
}