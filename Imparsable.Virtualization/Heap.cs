using System.Runtime.InteropServices;

namespace Imparsable.Virtualization;

public sealed class Heap<TEntry>(Memory<byte> memory) where TEntry : unmanaged, IHeapEntry
{
    private const int Alignment = sizeof(long);
    private readonly List<TEntry> _entries = new(256);
    private int _pointer;

    public int Allocate(int size, TEntry entry)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        size = Align(size);

        if (_pointer > memory.Length - size)
            throw new OutOfMemoryException();

        entry.Offset = _pointer;
        entry.Size = size;

        var handle = _entries.Count;
        _entries.Add(entry);
        _pointer += size;

        return handle;
    }

    public Span<byte> GetBytes(int handle)
    {
        ref var entry = ref GetEntry(handle);
        return memory.Span.Slice(entry.Offset, entry.Size);
    }

    public ref TEntry GetEntry(int handle)
    {
        if ((uint)handle >= (uint)_entries.Count)
            throw new ArgumentOutOfRangeException(nameof(handle));

        return ref CollectionsMarshal.AsSpan(_entries)[handle];
    }

    private static int Align(int size) => (size + Alignment - 1) & ~(Alignment - 1);
}