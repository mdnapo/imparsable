using Imparsable.Tools.Virtualization;

namespace Imparsable.Lang.Calculator.Virtualization;

public class VirtualMemory : VirtualMemory<StackSlot, VirtualMemoryHeapEntry>
{
    public StringHeap StringHeap { get; }

    public VirtualMemory()
    {
        StringHeap = new StringHeap(Heap);
    }
}