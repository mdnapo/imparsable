namespace Imparsable.Toolchain.Compilation;

public readonly struct Chunk(ReadOnlyMemory<byte> code, ReadOnlyMemory<byte> constants)
{
    public ReadOnlySpan<byte> Code => code.Span;
    public ReadOnlySpan<byte> Constants => constants.Span;
}