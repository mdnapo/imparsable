namespace Imparsable.Tools.Parsing.Attributes;

public sealed class SequenceAttribute<TToken>(string @string) : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override int Priority => 60;
    
    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int line = src.Line, column = src.Column;

        if (!src.Match(@string)) return false;

        var range = src.Extract();
        
        context.AddToken(Type, range.Offset, range.Length, line, column);

        return true;
    }
}