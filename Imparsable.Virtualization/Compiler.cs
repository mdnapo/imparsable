using System.Buffers;
using System.Buffers.Binary;

namespace Imparsable.Virtualization;

public abstract class Compiler<T> where T : unmanaged
{
    static Compiler()
    {
        if (Enum.GetUnderlyingType(typeof(T)) != typeof(byte))
            throw new InvalidOperationException($"Enum {typeof(T).Name} is not backed by byte.");
    }

    public List<byte> Code { get; } = [];
    public List<byte> Constants { get; } = [];

    protected byte[] AcquireBuffer(int size) => ArrayPool<byte>.Shared.Rent(size);
    protected void ReleaseBuffer(byte[] buffer) => ArrayPool<byte>.Shared.Return(buffer);

    public Compiler<T> EmitOpCode(T op)
    {
        Code.Add((byte)(object)op);
        return this;
    }

    public int AddConstant(ReadOnlySpan<byte> value)
    {
        var offset = Constants.Count;
        Constants.AddRange(value);
        return offset;
    }

    public Compiler<T> EmitInt32(int value)
    {
        var buffer = AcquireBuffer(sizeof(int));
        var span = buffer.AsSpan()[..sizeof(int)];
        
        BinaryPrimitives.WriteInt32LittleEndian(span, value);
        Code.AddRange(span);
        ReleaseBuffer(buffer);
        
        return this;
    }

    public int EmitJump(T instruction)
    {
        EmitOpCode(instruction);
        EmitInt32(0);
        return Code.Count - sizeof(int);
    }

    public void EmitLoop(T jump, int loopStart)
    {
        // Since we're looping, we pass a negative offset to the JUMP instruction.
        EmitOpCode(jump);
        // Account for the parameter of the jump instruction by subtracting sizeof(int) from the offset.
        var offset = -(Code.Count - loopStart + sizeof(int));
        EmitInt32(offset);
    }

    public void PatchJump(int offset)
    {
        var jump = Code.Count - offset - sizeof(int);
        var buffer = AcquireBuffer(sizeof(int));
        var span = buffer.AsSpan()[..sizeof(int)];

        BinaryPrimitives.WriteInt32LittleEndian(span, jump);
        Code[offset + 0] = span[0]; // + 0  is not necessary, but this just looks better in terms of consistency
        Code[offset + 1] = span[1];
        Code[offset + 2] = span[2];
        Code[offset + 3] = span[3];

        ReleaseBuffer(buffer);
    }
}