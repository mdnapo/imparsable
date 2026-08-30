using System.Buffers;
using System.Runtime.InteropServices;

namespace Imparsable.Toolchain.Virtualization;

public sealed partial class Heap<TAllocation>(Memory<byte> memory) where TAllocation : unmanaged, IAllocation
{
    private const int Alignment = sizeof(long);
    private readonly List<TAllocation> _allocations = new(128);
    private readonly System.Collections.Generic.Stack<int> _reclaimed = new(128);
    private int _pointer;

    public Span<TAllocation> Allocations => CollectionsMarshal.AsSpan(_allocations);
    public int Pointer => _pointer;

    public int Allocate(int size, TAllocation entry)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        size = Align(size);

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

    private int AllocateHandle(ref TAllocation entry)
    {
        if (_reclaimed.TryPop(out var handle))
        {
            _allocations[handle] = entry;
            return handle;
        }

        handle = _allocations.Count;
        _allocations.Add(entry);

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

    public void Reclaim()
    {
        for (var index = 0; index < Allocations.Length; index++)
        {
            ref var allocation = ref Allocations[index];
            if (!allocation.IsMarked)
            {
                Reclaim(index, ref allocation);
            }
            else
            {
                allocation.IsMarked = false;
            }
        }
    }

    private void Reclaim(int handle, ref TAllocation allocation)
    {
        allocation.IsAllocated = false;
        allocation.Offset = 0;
        allocation.Size = 0;
        _reclaimed.Push(handle);
    }

    public void Compress()
    {
        var allocations = Allocations;
        var map = ArrayPool<CompressionIndex>.Shared.Rent(allocations.Length);
        var count = GetCompressionMap(allocations, ref map);

        try
        {
            var destination = 0;

            for (var index = 0; index < count; index++)
            {
                ref var allocation = ref allocations[map[index].Handle];

                if (allocation.Offset != destination)
                    Move(destination, ref allocation);

                destination += allocation.Size;
            }

            _pointer = destination;
        }
        finally
        {
            ArrayPool<CompressionIndex>.Shared.Return(map);
        }
    }

    private static int GetCompressionMap(Span<TAllocation> allocations, ref CompressionIndex[] map)
    {
        var index = 0;

        for (var handle = 0; handle < allocations.Length; handle++)
        {
            var allocation = allocations[handle];

            if (!allocation.IsAllocated)
                continue;

            map[index++] = new CompressionIndex(handle, allocation.Offset);
        }

        Array.Sort(map, 0, index, CompressionIndex.Comparer.Instance);

        return index;
    }

    private void Move(int destination, ref TAllocation allocation)
    {
        var allocationSpan = memory.Span.Slice(allocation.Offset, allocation.Size);
        var destinationSpan = memory.Span.Slice(destination, allocation.Size);
        allocationSpan.CopyTo(destinationSpan);
        allocation.Offset = destination;
    }

    private static int Align(int size) => (size + Alignment - 1) & ~(Alignment - 1);
}