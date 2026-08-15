namespace Imparsable.Parsing.Attributes;

public sealed class IdentifierAttribute<TToken> : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;

        if (!IsAlpha(src.Peek())) return false;

        int line = src.Line, column = src.Column;

        while (IsAlphaNumeric(src.Peek())) src.Advance();

        var range = src.Extract();

        context.AddToken(Type, range.Offset, range.Length, line, column);

        return true;
    }
}