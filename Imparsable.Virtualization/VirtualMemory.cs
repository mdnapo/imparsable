using System.Runtime.InteropServices;

namespace Imparsable.Virtualization;

public sealed class VirtualMemory<TStackSlot, THeapEntry>
    where TStackSlot : unmanaged
    where THeapEntry : unmanaged, IVirtualMemoryHeapEntry
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly Memory<byte> _memory;

    public VirtualMemoryStack<TStackSlot> Stack { get; }
    public VirtualMemoryHeap<THeapEntry> Heap { get; }

    public VirtualMemory(int stackSlots = 256, int heapMemoryInMbs = 8)
    {
        var stackSize = stackSlots * Marshal.SizeOf<TStackSlot>();
        var heapSize = heapMemoryInMbs * 1024 * 1024;

        _memory = new byte[stackSize + heapSize];
        Stack = new VirtualMemoryStack<TStackSlot>(_memory[..stackSize]);
        Heap = new VirtualMemoryHeap<THeapEntry>(_memory.Slice(stackSize, heapSize));
    }
}