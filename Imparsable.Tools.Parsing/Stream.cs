namespace Imparsable.Tools.Parsing;

public abstract class Stream<T>(ReadOnlyMemory<T> tokens)
{
    protected ReadOnlySpan<T> Sequence => tokens.Span;
    protected int Position { get; set; }

    public T Current => Position < Sequence.Length
        ? Sequence[Position]
        : default!;

    public virtual T Advance() => Position + 1 <= Sequence.Length
        ? Sequence[++Position - 1]
        : default!;

    public virtual T Peek(int offset = 0) => Position + offset < Sequence.Length
        ? Sequence[Position + offset]
        : default!;

    public virtual bool Ended() => Position >= Sequence.Length;
}