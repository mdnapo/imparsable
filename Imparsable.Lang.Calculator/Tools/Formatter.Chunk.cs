namespace Imparsable.Lang.Calculator.Tools;

public partial class Formatter
{
    private readonly record struct Chunk(int Offset, int Length, Split SplitBefore, int Depth);
}