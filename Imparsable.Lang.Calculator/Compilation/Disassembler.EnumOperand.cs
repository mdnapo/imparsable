using System.Text;

namespace Imparsable.Lang.Calculator.Compilation;

public sealed partial class Disassembler
{
    private sealed record EnumOperand<T>(T Value) : Operand where T : unmanaged
    {
        public override void WriteTo(StringBuilder builder, Instruction instruction) =>
            builder.Append(Value);
    }
}