namespace Imparsable.Tools.Virtualization.UnitTests;

internal struct Allocation : IAllocation
{
    public int Offset { get; set; }
    public int Size { get; set; }
    public bool IsAllocated { get; set; }
    public bool IsMarked { get; set; }
}