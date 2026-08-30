namespace Imparsable.Toolchain.Parsing.Interfaces;

public interface ILexerRule<TToken> where TToken : Enum
{
    public bool Match(Lexer<TToken>.Context context);
}