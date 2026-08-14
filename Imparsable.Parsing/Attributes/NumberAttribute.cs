namespace Imparsable.Parsing.Attributes;

public sealed class NumberAttribute<TToken> : LexerRuleAttribute<TToken> where TToken : Enum
{
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

        var lexeme = src.Extract();
        context.AddToken(Type, lexeme, line, column);

        return true;
    }
}