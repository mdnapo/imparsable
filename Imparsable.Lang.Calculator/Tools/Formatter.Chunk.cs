namespace Imparsable.Lang.Calculator.Tools;

public partial class Formatter
{
    private readonly record struct Chunk(string Text, Split SplitBefore, int Depth);
}