using System.Buffers.Binary;
using System.Text;
using Imparsable.Tools.Virtualization;

namespace Imparsable.Lang.Calculator.Virtualization;

public sealed class StringHeap(VirtualMemoryHeap<VirtualMemoryHeapEntry> heap)
{
    public int Allocate(ReadOnlySpan<char> value)
    {
        var byteCount = Encoding.UTF8.GetByteCount(value);

        var handle = heap.Allocate(
            sizeof(int) + byteCount,
            new VirtualMemoryHeapEntry { Type = HeapObjectType.STRING }
        );

        var data = heap.GetBytes(handle);

        BinaryPrimitives.WriteInt32LittleEndian(data, byteCount);

        Encoding.UTF8.GetBytes(value, data[sizeof(int)..]);

        return handle;
    }
}