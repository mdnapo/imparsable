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
        foreach (ref var slot in Stack.ActiveSlots)
            if (slot.Type is StackType.STRING)
                Heap.GetEntry(slot.Reference).IsMarked = true;
    }

    // TODO: Properly determine how often this should run.
    public void CollectGarbage()
    {
        MarkRoots();
        Heap.Reclaim();
        Heap.Compress();
    }
}