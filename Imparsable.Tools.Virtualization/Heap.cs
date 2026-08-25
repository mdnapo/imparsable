using System.Runtime.InteropServices;

namespace Imparsable.Tools.Virtualization;

public sealed class Heap<TAllocation>(Memory<byte> memory, Action collectGarbage)
    where TAllocation : unmanaged, IAllocation
{
    private const int Alignment = sizeof(long);
    private int _pointer;

    private readonly List<TAllocation> _allocations = new(256);
    private readonly System.Collections.Generic.Stack<int> _freed = new(128);
    public Span<TAllocation> Allocations => CollectionsMarshal.AsSpan(_allocations);

    public int Allocate(int size, TAllocation entry)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        size = Align(size);

        // TODO: Properly determine how often this should run.
        if (_pointer > memory.Length - size || true)
            collectGarbage.Invoke();

        if (_pointer > memory.Length - size)
            throw new OutOfMemoryException();

        entry.Offset = _pointer;
        entry.Size = size;
        entry.IsAllocated = true;
        entry.IsMarked = false;

        var handle = AllocateHandle(ref entry);

        _pointer += size;

        return handle;
    }

    public Span<byte> GetBytes(int handle)
    {
        ref var entry = ref GetEntry(handle);
        return memory.Span.Slice(entry.Offset, entry.Size);
    }

    public ref TAllocation GetEntry(int handle)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(handle, _allocations.Count);
        return ref Allocations[handle];
    }

    public void Compact()
    {
        var destination = 0;
        var entries = Allocations;

        for (var handle = 0; handle < entries.Length; handle++)
        {
            ref var entry = ref entries[handle];

            if (!entry.IsAllocated)
                continue;

            if (!entry.IsMarked)
            {
                Free(handle, ref entry);
                continue;
            }

            entry.IsMarked = false;

            if (entry.Offset != destination)
            {
                Move(destination, ref entry);
            }

            destination += entry.Size;
        }

        _pointer = destination;
    }

    private void Free(int handle, ref TAllocation allocation)
    {
        allocation.IsAllocated = false;
        allocation.Offset = 0;
        allocation.Size = 0;
        _freed.Push(handle);
    }

    private void Move(int destination, ref TAllocation allocation)
    {
        var allocationSpan = memory.Span.Slice(allocation.Offset, allocation.Size);
        var destinationSpan = memory.Span.Slice(destination, allocation.Size);
        allocationSpan.CopyTo(destinationSpan);
        allocation.Offset = destination;
    }

    private int AllocateHandle(ref TAllocation entry)
    {
        if (_freed.TryPop(out var handle))
        {
            _allocations[handle] = entry;
            return handle;
        }

        handle = _allocations.Count;
        _allocations.Add(entry);

        return handle;
    }

    private static int Align(int size) => (size + Alignment - 1) & ~(Alignment - 1);
}