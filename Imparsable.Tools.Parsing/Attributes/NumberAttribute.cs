namespace Imparsable.Tools.Parsing.Attributes;

public sealed class NumberAttribute<TToken> : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override int Priority => 50;
    
    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;

        if (!IsDigit(src.Peek())) return false;

        int line = src.Line, column = src.Column;

        ProcessDigits(src);

        if (IsDigit(src.Peek(1)) && src.Match('.'))
        {
            ProcessDigits(src);
        }

        var range = src.Extract();
        
        context.AddToken(Type, range.Offset, range.Length, line, column);

        return true;
    }
}