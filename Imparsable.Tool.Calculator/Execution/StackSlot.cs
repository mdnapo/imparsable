using System.Runtime.InteropServices;

namespace Imparsable.Tool.Calculator.Execution;

[StructLayout(LayoutKind.Explicit, Size = 16)]
public struct StackSlot
{
    [FieldOffset(0)]
    public StackType Type;

    [FieldOffset(8)]
    public bool Bool;

    [FieldOffset(8)]
    public double Number;

    [FieldOffset(8)]
    public int String;

    public override string ToString() => Type switch
    {
        StackType.BOOL => Bool.ToString(),
        StackType.NUMBER => Number.ToString(),
        _ => throw new InvalidOperationException()
    };
}