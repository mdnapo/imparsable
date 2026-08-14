namespace Imparsable.Parsing.Attributes;

public sealed class MultiCharacterAttribute<TToken>(string @string) : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int line = src.Line, column = src.Column;

        if (!src.Match(@string)) return false;

        var lexeme = src.Extract();
        context.AddToken(Type, lexeme, line, column);

        return true;
    }
}