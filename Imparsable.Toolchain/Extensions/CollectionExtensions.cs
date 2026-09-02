using System.Runtime.InteropServices;

namespace Imparsable.Toolchain.Extensions;

public static class CollectionExtensions
{
    extension<T>(List<T> list)
    {
        public Span<T> Span => CollectionsMarshal.AsSpan(list);
    }
}