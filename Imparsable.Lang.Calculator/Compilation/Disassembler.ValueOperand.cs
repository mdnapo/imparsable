using System.Text;

namespace Imparsable.Lang.Calculator.Compilation;

public sealed partial class Disassembler
{
    private sealed record ValueOperand(string Value) : Operand
    {
        public override void WriteTo(StringBuilder builder, Instruction instruction) =>
            builder.Append(Value);
    }
}