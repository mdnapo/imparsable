using System.Text;

namespace Imparsable.Lang.Calculator.Compilation;

public sealed partial class Disassembler
{
    private sealed record JumpOperand(int Offset) : Operand
    {
        public override void WriteTo(StringBuilder builder, Instruction instruction)
        {
            // TODO: Scrutinise this to make sure it works properly for all jumps.
            var offset = Offset + sizeof(byte) + sizeof(int);
            var target = instruction.Offset + sizeof(byte) + sizeof(int) + Offset;
            builder.Append(offset.ToString("+#;-#;0"));
            builder.Append(" -> ");
            builder.Append(target.ToString("000000"));
            
            // var target = instruction.Offset + sizeof(byte) + sizeof(int) + Offset;
            // builder.Append(Offset.ToString("+#;-#;0"));
            // builder.Append(" -> ");
            // builder.Append(target.ToString("000000"));
        }
    }
}