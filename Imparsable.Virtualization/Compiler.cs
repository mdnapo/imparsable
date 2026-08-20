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

    public void EmitOpCode(T op) => Code.Add((byte)(object)op);

    public int AddConstant(ReadOnlySpan<byte> value)
    {
        var offset = Constants.Count;
        Constants.AddRange(value);
        return offset;
    }

    public void EmitInt32(int value)
    {
        using var buffer = ByteBuffer.Acquire(sizeof(int));
        var span = buffer.Span;
        BinaryPrimitives.WriteInt32LittleEndian(span, value);
        Code.AddRange(span);
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
        using var buffer = ByteBuffer.Acquire(sizeof(int));
        var span = buffer.Span;
        BinaryPrimitives.WriteInt32LittleEndian(span, jump);

        Code[offset + 0] = span[0];
        Code[offset + 1] = span[1];
        Code[offset + 2] = span[2];
        Code[offset + 3] = span[3];
    }
}