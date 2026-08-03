namespace Imparsable.Parsing;

public partial class Parser<TToken>
{
    public class ContextProvider(Configuration configuration, List<Lexer<TToken>.Token> tokens)
    {
        public Context GetContext() => new(configuration, tokens);
    }
}