using System.Buffers;
using System.Runtime.InteropServices;

namespace Imparsable.Tools.Virtualization;

public sealed partial class Heap<TAllocation>(Memory<byte> memory) where TAllocation : unmanaged, IAllocation
{
    private const int Alignment = sizeof(long);
    private readonly List<TAllocation> _allocations = new(128);
    private readonly System.Collections.Generic.Stack<int> _reclaimed = new(128);
    private int _pointer;

    public Span<TAllocation> Allocations => CollectionsMarshal.AsSpan(_allocations);

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
            var allocation = Allocations[index];
            if (!allocation.IsAllocated)
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
        var buffer = ArrayPool<TAllocation>.Shared.Rent(allocations.Length);

        try
        {
            var count = 0;
            foreach (var allocation in allocations)
            {
                if (allocation.IsAllocated)
                    buffer[count++] = allocation;
            }

            Array.Sort(buffer, 0, count, AllocationOffsetComparer.Instance);

            var destination = 0;
            for (var i = 0; i < count; i++)
            {
                ref var entry = ref buffer[i];

                if (entry.Offset != destination)
                    Move(destination, ref entry);

                destination += entry.Size;
            }

            _pointer = destination;
        }
        finally
        {
            ArrayPool<TAllocation>.Shared.Return(buffer);
        }
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