using System.Runtime.InteropServices;

namespace Imparsable.Virtualization;

public class VirtualMemory<TStackSlot, THeapEntry>
    where TStackSlot : unmanaged
    where THeapEntry : unmanaged, IVirtualMemoryHeapEntry
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly Memory<byte> _memory;

    public VirtualMemoryStack<TStackSlot> Stack { get; }
    public VirtualMemoryHeap<THeapEntry> Heap { get; }

    public VirtualMemory(int stack = 256, int heap = 8)
    {
        var stackSegment = stack * Marshal.SizeOf<TStackSlot>();
        var heapSegment = heap * 1024 * 1024;

        _memory = new byte[stackSegment + heapSegment];
        Stack = new VirtualMemoryStack<TStackSlot>(_memory[..stackSegment]);
        Heap = new VirtualMemoryHeap<THeapEntry>(_memory.Slice(stackSegment, heapSegment));
    }
}