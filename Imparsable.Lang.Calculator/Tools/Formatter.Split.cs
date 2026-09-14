namespace Imparsable.Lang.Calculator.Tools;

public partial class Formatter
{
    private enum Split : byte
    {
        NONE,
        SPACE,
        LINE,
        BLANK_LINE,
        TWO_BLANK_LINES
    }
}