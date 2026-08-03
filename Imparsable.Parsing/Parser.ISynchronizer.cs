namespace Imparsable.Parsing;

public partial class Parser<TToken, TSyntax>
{
    public interface ISynchronizer
    {
        public static abstract bool Synchronize(Parser<TToken>.Context context);
    }
}