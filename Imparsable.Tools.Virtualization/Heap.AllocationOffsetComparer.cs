namespace Imparsable.Tools.Virtualization;

public sealed partial class Heap<TAllocation>
{
    private sealed class AllocationOffsetComparer : IComparer<TAllocation>
    {
        public static readonly AllocationOffsetComparer Instance = new();
        public int Compare(TAllocation x, TAllocation y) => x.Offset.CompareTo(y.Offset);
    }
}