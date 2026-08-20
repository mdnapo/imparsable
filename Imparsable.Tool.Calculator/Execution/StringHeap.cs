using System.Buffers.Binary;
using System.Text;
using Imparsable.Virtualization;

namespace Imparsable.Tool.Calculator.Execution;

public sealed class StringHeap(Heap<HeapEntry> heap)
{
    public int Allocate(ReadOnlySpan<char> value)
    {
        var byteCount = Encoding.UTF8.GetByteCount(value);

        var handle = heap.Allocate(
            sizeof(int) + byteCount,
            new HeapEntry { Type = HeapObjectType.STRING }
        );

        var data = heap.GetBytes(handle);

        BinaryPrimitives.WriteInt32LittleEndian(data, byteCount);

        Encoding.UTF8.GetBytes(value, data[sizeof(int)..]);

        return handle;
    }
}