using Imparsable.Virtualization;

namespace Imparsable.Tool.Calculator.Execution;

public class VirtualMemory : VirtualMemory<StackSlot, VirtualMemoryHeapEntry>
{
    public StringHeap StringHeap { get; }

    public VirtualMemory()
    {
        StringHeap = new StringHeap(Heap);
    }
}