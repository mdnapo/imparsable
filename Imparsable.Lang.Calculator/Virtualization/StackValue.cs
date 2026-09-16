using System.Runtime.InteropServices;

namespace Imparsable.Lang.Calculator.Virtualization;

[StructLayout(LayoutKind.Explicit, Size = 16)]
public struct StackValue
{
    [FieldOffset(0)]
    public bool Bool;

    [FieldOffset(0)]
    public double Number;

    [FieldOffset(0)]
    public int Reference;

    [FieldOffset(8)]
    public StackType Type;

    public static StackValue FromBool(bool value) => new() { Bool = value, Type = StackType.BOOL };
    public static StackValue FromNumber(double value) => new() { Number = value, Type = StackType.NUMBER };
    public static StackValue FromString(int value) => new() { Reference = value, Type = StackType.STRING };
}