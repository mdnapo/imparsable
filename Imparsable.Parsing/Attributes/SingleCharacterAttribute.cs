namespace Imparsable.Parsing.Attributes;

public sealed class SingleCharacterAttribute<TToken>(char @char) : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int line = src.Line, column = src.Column;

        if (!src.Match(@char)) return false;

        var range = src.Extract();
        
        context.AddToken(Type, range.Offset, range.Length, line, column);

        return true;
    }
}