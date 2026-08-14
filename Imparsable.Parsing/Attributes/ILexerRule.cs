namespace Imparsable.Parsing.Attributes;

public interface ILexerRule<TToken> where TToken : Enum
{
    public TToken Type { get; }
    public bool Match(Lexer<TToken>.Context context);
}