using Imparsable.Tools.Virtualization;

namespace Imparsable.Lang.Calculator.Virtualization;

public class Memory : Memory<StackSlot, Allocation>
{
    public StringHeap StringHeap { get; }

    public Memory()
    {
        StringHeap = new StringHeap(Heap);
    }

    private void MarkRoots()
    {
        foreach (ref var slot in Stack.Slots)
            if (slot.Type is StackType.STRING)
                Heap.GetEntry(slot.Reference).IsMarked = true;
    }

    public override void CollectGarbage()
    {
        MarkRoots();
        Heap.Compact();
    }
}