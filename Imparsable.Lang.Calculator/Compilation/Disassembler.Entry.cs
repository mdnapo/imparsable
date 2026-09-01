using System.Text;

namespace Imparsable.Lang.Calculator.Compilation;

public sealed partial class Disassembler
{
    private abstract record Entry(int Indent)
    {
        public abstract void WriteTo(StringBuilder builder);

        protected void WriteIndent(StringBuilder builder) =>
            builder.Append(' ', Indent * 2);
    }
}