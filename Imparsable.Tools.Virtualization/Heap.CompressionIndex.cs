namespace Imparsable.Tools.Virtualization;

public sealed partial class Heap<TAllocation>
{
    private readonly record struct CompressionIndex(int Handle, int Offset)
    {
        public sealed class Comparer : IComparer<CompressionIndex>
        {
            public static readonly Comparer Instance = new();
            public int Compare(CompressionIndex x, CompressionIndex y) => x.Offset - y.Offset;
        }
    }
}