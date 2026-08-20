using System.Runtime.InteropServices;

namespace Imparsable.Virtualization;

public sealed class VirtualMemoryStack<TSlot>(Memory<byte> memory) where TSlot : unmanaged
{
    public Span<TSlot> Slots => MemoryMarshal.Cast<byte, TSlot>(memory.Span);

    private int Pointer { get; set; }

    public void Push(TSlot value)
    {
        if (Pointer >= Slots.Length - 1)
            throw new StackOverflowException("A stack overflow occurred.");

        Slots[Pointer++] = value;
    }

    public TSlot Pop()
    {
        return Pointer == 0
            ? throw new InvalidOperationException("Cannot pop an empty stack.")
            : Slots[--Pointer];
    }

    public ref TSlot Peek()
    {
        if (Pointer == 0)
            throw new InvalidOperationException("Cannot peek an empty stack.");

        return ref Slots[Pointer - 1];
    }
}