namespace Imparsable.Virtualization;

public interface IHeapEntry
{
    int Offset { get; set; }
    int Size { get; set; }
}