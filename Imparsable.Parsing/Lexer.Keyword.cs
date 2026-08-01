namespace Imparsable.Parsing;

public abstract partial class Lexer<TToken>
{
    public class Keyword
    {
        public required string Name { get; init; }
        public required TToken Type { get; init; }
    }
}