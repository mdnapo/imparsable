using System.Runtime.InteropServices;

namespace Imparsable.Tools.Virtualization.UnitTests;

[StructLayout(LayoutKind.Explicit, Size = sizeof(int))]
internal struct StackSlot
{
    [FieldOffset(0)]
    public int Number;

    public static StackSlot FromNumber(int value) => new() { Number = value };
}