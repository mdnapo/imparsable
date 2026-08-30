namespace Imparsable.Toolchain.Parsing.Interfaces;

public interface ISyntax<TToken> where TToken : Enum
{
    public Lexer<TToken>.Token Token { get; }

    public delegate TSyntax Func<out TSyntax>(ParserContext<TToken> context) where TSyntax : ISyntax<TToken>;
}