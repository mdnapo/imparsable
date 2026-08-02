namespace Imparsable.Parsing;

public static partial class Parser<TToken> where TToken : Enum
{
    public interface IProduction<out TSyntax> where TSyntax : ISyntax<TToken>
    {
        public static abstract TSyntax Parse(Context context);
    }
}