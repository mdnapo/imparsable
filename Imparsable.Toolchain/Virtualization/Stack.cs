using System.Runtime.InteropServices;

namespace Imparsable.Toolchain.Virtualization;

public sealed class Stack<TValue>(Memory<byte> memory) where TValue : unmanaged
{
    public Span<TValue> Slots => MemoryMarshal.Cast<byte, TValue>(memory.Span);
    public Span<TValue> ActiveSlots => MemoryMarshal.Cast<byte, TValue>(memory.Span)[..Pointer];

    private int Pointer { get; set; }

    public void Push(TValue value)
    {
        if (Pointer >= Slots.Length)
            throw new StackOverflowException("A stack overflow occurred.");

        Slots[Pointer++] = value;
    }

    public TValue Pop()
    {
        if (Pointer == 0)
            throw new InvalidOperationException("Cannot pop an empty stack.");

        ref var slot = ref Slots[--Pointer];
        var value = slot;
        slot = default;

        return value;
    }

    public ref TValue Peek()
    {
        if (Pointer == 0)
            throw new InvalidOperationException("Cannot peek an empty stack.");

        return ref Slots[Pointer - 1];
    }
}