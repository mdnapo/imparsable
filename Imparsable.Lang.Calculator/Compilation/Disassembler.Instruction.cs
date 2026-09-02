using System.Text;

namespace Imparsable.Lang.Calculator.Compilation;

public sealed partial class Disassembler
{
    private sealed record Instruction(int Offset, int Indent, OpCode OpCode) : Entry(Indent)
    {
        public List<Operand> Operands { get; } = [];

        public override void WriteTo(StringBuilder builder)
        {
            WriteIndent(builder);
            builder.Append(Offset.ToString("000000"));
            builder.Append(' ');
            builder.Append(OpCode);

            foreach (var operand in Operands)
            {
                builder.Append(' ');
                operand.WriteTo(builder, this);
            }

            builder.AppendLine();
        }
    }
}