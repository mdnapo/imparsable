using System.Runtime.InteropServices;

namespace Imparsable.Tool.Calculator.Execution;

[StructLayout(LayoutKind.Explicit, Size = 8)]
public struct StackSlot
{
    [FieldOffset(0)]
    public bool Bool;

    [FieldOffset(0)]
    public double Number;

    [FieldOffset(0)]
    public int String;

    public static StackSlot FromBool(bool value) => new() { Bool = value };
    public static StackSlot FromNumber(double value) => new() { Number = value };
    public static StackSlot FromString(int value) => new() { String = value };
}