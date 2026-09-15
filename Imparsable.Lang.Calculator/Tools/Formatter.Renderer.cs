using System.Text;
using Imparsable.Toolchain.Parsing;

namespace Imparsable.Lang.Calculator.Tools;

public partial class Formatter
{
    private sealed class Renderer(Source source, int tabSize)
    {
        public string Render(Document document)
        {
            var builder = new StringBuilder();

            foreach (var chunk in document)
            {
                WriteSplit(builder, chunk.SplitBefore);

                if (builder.Length == 0 || chunk.SplitBefore >= Split.LINE)
                    builder.Append(' ', chunk.Depth * tabSize);

                builder.Append(source.GetTextSpan(chunk.Offset, chunk.Length));
            }

            builder.Append(Environment.NewLine);

            return builder.ToString();
        }

        private static void WriteSplit(StringBuilder builder, Split split)
        {
            switch (split)
            {
                case Split.SPACE:
                {
                    builder.Append(' ');
                    break;
                }

                case Split.LINE:
                {
                    builder.Append(Environment.NewLine);
                    break;
                }

                case Split.BLANK_LINE:
                {
                    builder.Append(Environment.NewLine);
                    builder.Append(Environment.NewLine);
                    break;
                }

                case Split.TWO_BLANK_LINES:
                {
                    builder.Append(Environment.NewLine);
                    builder.Append(Environment.NewLine);
                    builder.Append(Environment.NewLine);
                    break;
                }
            }
        }
    }
}