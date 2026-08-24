using Imparsable.Tools.Virtualization;

namespace Imparsable.Lang.Calculator.Virtualization;

public struct VirtualMemoryHeapEntry : IVirtualMemoryHeapEntry
{
    public int Offset { get; set; }
    public int Size { get; set; }
    public int TypeId { get; init; }
    public HeapObjectType Type { get; init; }
}