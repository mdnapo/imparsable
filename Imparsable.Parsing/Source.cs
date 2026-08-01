namespace Imparsable.Parsing;

public class Source(string file, string source) : Stream<char>(source.AsMemory()), ISourceMarker
{
    public string File { get; } = file;
    public int Start { get; set; }
    public int Line { get; set; } = 1;
    public int Column { get; set; } = 1;
    public char Last => Tokens[^1];

    public override char Advance()
    {
        Column++;
        return base.Advance();
    }

    public override bool Ended() => Current == '\0' || base.Ended();

    public string Extract()
    {
        var position = (Start, Length: Position - Start);
        Start = Position;
        return source.Substring(position.Start, position.Length);
    }

    public void Ignore()
    {
        Start = Position;
    }

    public bool Match(char expected)
    {
        if (Tokens[Position] != expected) return false;
        Position += 1;
        return true;
    }

    public bool Match(ReadOnlySpan<char> expected)
    {
        if (Position + expected.Length > Tokens.Length) return false;

        for (var index = 0; index < expected.Length; index++)
        {
            if (Tokens[Position + index] != expected[index])
            {
                return false;
            }
        }

        Position += expected.Length;

        return true;
    }
}