namespace Imparsable.Toolchain.Parsing.Attributes;

public sealed class CharacterAttribute<TToken>(char @char) : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override int Priority => 70;
    
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