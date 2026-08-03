namespace Imparsable.Parsing;

public partial class Parser<TToken, TSyntax>
{
    public interface IProduction
    {
        public static abstract TSyntax Parse(Parser<TToken>.Context context);
    }
}