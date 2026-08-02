namespace Imparsable.Parsing;

public class Parser<TToken, TSyntax>(
    Lexer<TToken>.Context ctx,
    List<TSyntax> syntax,
    Parser<TToken, TSyntax>.IProduction entrypoint
)
    where TToken : Enum
    where TSyntax : ISyntax<TToken>
{
    public interface IProduction
    {
        public static abstract TSyntax Parse(Parser<TToken>.Context context);
    }

    public List<ISyntax<TToken>> Execute()
    {
        throw new NotImplementedException();
    }
}