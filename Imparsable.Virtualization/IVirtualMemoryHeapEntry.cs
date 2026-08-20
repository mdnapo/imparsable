namespace Imparsable.Virtualization;

public interface IVirtualMemoryHeapEntry
{
    int Offset { get; set; }
    int Size { get; set; }
}