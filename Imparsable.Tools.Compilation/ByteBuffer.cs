using System.Buffers;

namespace Imparsable.Tools.Compilation;

public static class ByteBuffer
{
    public readonly record struct Handle(int Size) : IDisposable
    {
        private readonly byte[] _buffer = ArrayPool<byte>.Shared.Rent(Size);
        public Span<byte> Span => _buffer.AsSpan(0, Size);
        public void Dispose() => ArrayPool<byte>.Shared.Return(_buffer);
    }

    public static Handle Acquire(int size) => new(size);
}