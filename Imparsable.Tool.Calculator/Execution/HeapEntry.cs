using Imparsable.Virtualization;

namespace Imparsable.Tool.Calculator.Execution;

public struct HeapEntry : IHeapEntry
{
    public int Offset { get; set; }
    public int Size { get; set; }
    public int TypeId { get; init; }
    public HeapObjectType Type { get; init; }
}