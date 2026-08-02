namespace Imparsable.Parsing;

public interface ISyntax<TToken> where TToken : Enum
{
    public Lexer<TToken>.Token Token { get; }

    public delegate TSyntax Func<out TSyntax>(Parser<TToken>.Context context) where TSyntax : ISyntax<TToken>;
}