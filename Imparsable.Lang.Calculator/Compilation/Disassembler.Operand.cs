using System.Text;

namespace Imparsable.Lang.Calculator.Compilation;

public sealed partial class Disassembler
{
    private abstract record Operand
    {
        public abstract void WriteTo(StringBuilder builder, Instruction instruction);
    }
}