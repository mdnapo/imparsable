namespace Imparsable.Parsing;

public abstract class Stream<T>(ReadOnlyMemory<T> tokens)
{
    protected ReadOnlySpan<T> Tokens => tokens.Span;
    protected int Position { get; set; }

    public T Current => Position < Tokens.Length
        ? Tokens[Position]
        : default!;

    public virtual T Advance() => Position + 1 <= Tokens.Length
        ? Tokens[++Position - 1]
        : default!;

    public virtual T Peek(int offset = 0) => Position + offset < Tokens.Length
        ? Tokens[Position + offset]
        : default!;

    public virtual bool Ended() => Position >= Tokens.Length;
}