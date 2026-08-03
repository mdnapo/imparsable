namespace Imparsable.Parsing;

public class Parser<TToken, TSyntax>(
    Lexer<TToken> lexer,
    Parser<TToken>.ContextProvider contextProvider, 
    List<TSyntax> syntax
)
    where TToken : Enum
    where TSyntax : ISyntax<TToken>
{
    public interface IProduction
    {
        public static abstract TSyntax Parse(Parser<TToken>.Context context);
    }

    public List<TSyntax> Execute<TProduction>() where TProduction : IProduction
    {
        lexer.Execute();
        
        var context = contextProvider.GetContext();
        
        while (!context.Ended())
            syntax.Add(TProduction.Parse(context));

        return syntax;
    }
}