using System.Runtime.InteropServices;

namespace Imparsable.Toolchain.Virtualization;

public sealed class Stack<TSlot>(Memory<byte> memory) where TSlot : unmanaged
{
    public Span<TSlot> Slots => MemoryMarshal.Cast<byte, TSlot>(memory.Span);
    public Span<TSlot> ActiveSlots => MemoryMarshal.Cast<byte, TSlot>(memory.Span)[..Pointer];

    private int Pointer { get; set; }

    public void Push(TSlot value)
    {
        if (Pointer >= Slots.Length)
            throw new StackOverflowException("A stack overflow occurred.");

        Slots[Pointer++] = value;
    }

    public TSlot Pop()
    {
        if (Pointer == 0)
            throw new InvalidOperationException("Cannot pop an empty stack.");

        ref var slot = ref Slots[--Pointer];
        var value = slot;
        slot = default;

        return value;
    }

    public ref TSlot Peek()
    {
        if (Pointer == 0)
            throw new InvalidOperationException("Cannot peek an empty stack.");

        return ref Slots[Pointer - 1];
    }
}