using System.Buffers.Binary;
using System.Text;
using Imparsable.Tools.Virtualization;

namespace Imparsable.Lang.Calculator.Virtualization;

public sealed class StringHeap(Heap<Allocation> heap)
{
    public int Allocate(ReadOnlySpan<char> value)
    {
        var length = Encoding.UTF8.GetByteCount(value);
        var handle = heap.Allocate(sizeof(int) + length, new Allocation());
        var data = heap.GetBytes(handle);

        BinaryPrimitives.WriteInt32LittleEndian(data, length);
        Encoding.UTF8.GetBytes(value, data[sizeof(int)..(sizeof(int) + length)]);

        return handle;
    }

    public int Allocate(ReadOnlySpan<byte> value)
    {
        var length = value.Length;
        var handle = heap.Allocate(sizeof(int) + length, new Allocation());
        var destination = heap.GetBytes(handle);

        BinaryPrimitives.WriteInt32LittleEndian(destination, length);

        var payload = destination[sizeof(int)..];
        value.CopyTo(payload);

        return handle;
    }

    public int Allocate(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
    {
        var length = checked(left.Length + right.Length);
        var handle = heap.Allocate(sizeof(int) + length, new Allocation());
        var destination = heap.GetBytes(handle);

        BinaryPrimitives.WriteInt32LittleEndian(destination, length);

        var payload = destination[sizeof(int)..];
        left.CopyTo(payload);
        right.CopyTo(payload[left.Length..]);

        return handle;
    }

    public ReadOnlySpan<byte> GetValueUtf8(int handle)
    {
        var bytes = heap.GetBytes(handle);
        var length = BinaryPrimitives.ReadInt32LittleEndian(bytes[..sizeof(int)]);
        return bytes.Slice(sizeof(int), length);
    }
}