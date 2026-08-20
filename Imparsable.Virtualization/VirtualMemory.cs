using System.Runtime.InteropServices;

namespace Imparsable.Virtualization;

public sealed class VirtualMemory<TStackSlot, THeapEntry>
    where TStackSlot : unmanaged
    where THeapEntry : unmanaged, IHeapEntry
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly Memory<byte> _memory;

    public Stack<TStackSlot> Stack { get; }
    public Heap<THeapEntry> Heap { get; }

    public VirtualMemory(int stackSlots = 256, int heapMemoryInMbs = 8)
    {
        var stackSize = stackSlots * Marshal.SizeOf<TStackSlot>();
        var heapSize = heapMemoryInMbs * 1024 * 1024;

        _memory = new byte[stackSize + heapSize];
        Stack = new Stack<TStackSlot>(_memory[..stackSize]);
        Heap = new Heap<THeapEntry>(_memory.Slice(stackSize, heapSize));
    }
}