namespace Imparsable.Toolchain.Virtualization;

public interface IAllocation
{
    int Offset { get; set; }
    int Size { get; set; }
    bool IsAllocated { get; set; }
    bool IsMarked { get; set; }
}