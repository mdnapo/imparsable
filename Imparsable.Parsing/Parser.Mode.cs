namespace Imparsable.Parsing;

public abstract partial class Parser<TToken>
{
    public enum Mode
    {
        Strict = 0,
        Tolerant = 1,
    }
}