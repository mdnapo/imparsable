namespace Imparsable.Parsing.Interfaces;

public interface ISyntaxTree<TToken, TSyntax, out TSyntaxTree>
    where TToken : Enum
    where TSyntax : ISyntax<TToken>
    where TSyntaxTree : SyntaxTree<TToken, TSyntax, TSyntaxTree>, ISyntaxTree<TToken, TSyntax, TSyntaxTree>, new()
{
    public static abstract TSyntaxTree Create(string source);
}