using System.Runtime.InteropServices;

namespace Imparsable.Toolchain.Virtualization;

public class Memory<TStackSlot, THeapEntry>
    where TStackSlot : unmanaged
    where THeapEntry : unmanaged, IAllocation
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly Memory<byte> _memory;

    public Stack<TStackSlot> Stack { get; }
    public Heap<THeapEntry> Heap { get; }

    public Memory(int stack = 256, int heap = 8)
    {
        var stackSegment = stack * Marshal.SizeOf<TStackSlot>();
        var heapSegment = heap * 1024 * 1024;

        _memory = new byte[stackSegment + heapSegment];
        Stack = new Stack<TStackSlot>(_memory[..stackSegment]);
        Heap = new Heap<THeapEntry>(_memory.Slice(stackSegment, heapSegment));
    }
}