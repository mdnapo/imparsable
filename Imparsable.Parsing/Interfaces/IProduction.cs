namespace Imparsable.Parsing.Interfaces;

public interface IProduction<TToken, TSyntax>
    where TToken : Enum
    where TSyntax : ISyntax<TToken>
{
    public static abstract TSyntax Parse(ParserContext<TToken> context);
}