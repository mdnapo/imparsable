using Imparsable.Parsing.Interfaces;

namespace Imparsable.Parsing.Attributes;

[AttributeUsage(validOn: AttributeTargets.Field)]
public abstract class LexerRuleAttribute<TToken> : Attribute, ILexerRule<TToken> where TToken : Enum
{
    public TToken Type { get; private set; } = default!;
    public abstract bool Match(Lexer<TToken>.Context context);

    protected static bool IsAlpha(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';

    protected static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

    protected static bool IsDigit(char c) => c is >= '0' and <= '9';

    protected static void ProcessDigits(Source src)
    {
        while (IsDigit(src.Peek()))
            src.Advance();
    }

    public LexerRuleAttribute<TToken> SetType(TToken type)
    {
        Type = type;
        return this;
    }

    protected void HandleIgnore(
        bool ignore,
        TToken type,
        Lexer<TToken>.Context context,
        Source src,
        int line,
        int column
    )
    {
        if (ignore)
        {
            src.Ignore();
        }
        else
        {
            var lexeme = src.Extract();
            context.AddToken(type, lexeme, line, column);
        }
    }
}