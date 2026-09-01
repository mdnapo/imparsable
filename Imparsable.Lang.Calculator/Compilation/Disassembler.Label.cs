using System.Text;

namespace Imparsable.Lang.Calculator.Compilation;

public sealed partial class Disassembler
{
    private sealed record Label(int Indent, string Name) : Entry(Indent)
    {
        public override void WriteTo(StringBuilder builder)
        {
            WriteIndent(builder);
            builder.Append(Name);
            builder.AppendLine();
        }
    }
}