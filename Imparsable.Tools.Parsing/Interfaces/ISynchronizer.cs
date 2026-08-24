namespace Imparsable.Tools.Parsing.Interfaces;

public interface ISynchronizer<TToken> where TToken : Enum
{
    public static abstract bool Synchronize(ParserContext<TToken> context);
}