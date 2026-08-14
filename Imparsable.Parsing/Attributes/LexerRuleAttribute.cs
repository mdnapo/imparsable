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

public sealed class DoubleQuoteStringAttribute<TToken> : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override bool Match(Lexer<TToken>.Context context)
    {
        if (!context.Source.Match('"')) return false;

        var src = context.Source;
        int line = src.Line, column = src.Column;

        while (src.Peek() != '"' && !src.Ended())
        {
            // Account for source lines and columns when dealing with a string spanning multiple lines.
            if (src.Peek() == '\n')
            {
                src.Line++;
                src.Column = 1;
            }

            src.Advance();
        }

        if (src.Ended())
        {
            context.Halt($"Unterminated string near '{src.Last}'.");
        }

        // Include the closing quotation mark.
        src.Advance();

        var lexeme = src.Extract();
        context.AddToken(Type, lexeme.Trim('"'), line, column);

        return true;
    }
}

public sealed class IdentifierAttribute<TToken> : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;

        if (!IsAlpha(src.Peek())) return false;

        int line = src.Line, column = src.Column;

        while (IsAlphaNumeric(src.Peek())) src.Advance();

        var lexeme = src.Extract();

        context.AddToken(Type, lexeme, line, column);

        return true;
    }
}

public sealed class WhitespaceAttribute<TToken>(bool ignore = true) : LexerRuleAttribute<TToken> where TToken : Enum
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly char[] WhitespaceOrCarriageReturn = [' ', '\r'];

    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int line = src.Line, column = src.Column;

        if (src.MatchAny(WhitespaceOrCarriageReturn))
        {
            while (src.CheckAny(WhitespaceOrCarriageReturn) && !src.Ended())
                src.Advance();

            HandleIgnore(ignore, Type, context, src, line, column);

            return true;
        }

        if (src.Match('\t'))
        {
            // We do -1 because the call to Match already increments src.Column by 1.
            do src.Column += context.Configuration.TabSize - 1;
            while (src.Match('\t') && !src.Ended());

            HandleIgnore(ignore, Type, context, src, line, column);

            return true;
        }

        return false;
    }
}

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

public sealed class NewLineAttribute<TToken>(bool ignore = true) : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int line = src.Line, column = src.Column;

        if (!src.Match('\n')) return false;

        while (src.Check('\n') && !src.Ended())
            src.Advance();

        HandleIgnore(ignore, Type, context, src, line, column);

        context.Source.Line++;
        context.Source.Column = 1;

        return true;
    }
}

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

public sealed class SingleCharacterAttribute<TToken>(char @char) : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override bool Match(Lexer<TToken>.Context context)
    {
        var src = context.Source;
        int line = src.Line, column = src.Column;

        if (!src.Match(@char)) return false;

        var lexeme = src.Extract();
        context.AddToken(Type, lexeme, line, column);

        return true;
    }
}

public sealed class SingleQuoteStringAttribute<TToken> : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override bool Match(Lexer<TToken>.Context context)
    {
        if (!context.Source.Match('\'')) return false;

        var src = context.Source;
        int line = src.Line, column = src.Column;

        while (src.Peek() != '\'' && !src.Ended())
        {
            // Account for source lines and columns when dealing with a string spanning multiple lines.
            if (src.Peek() == '\n')
            {
                src.Line++;
                src.Column = 1;
            }

            src.Advance();
        }

        if (src.Ended())
        {
            context.Halt($"Unterminated string near '{src.Last}'.");
        }

        // Include the closing quotation mark.
        src.Advance();

        var lexeme = src.Extract();

        context.AddToken(Type, lexeme.Trim('\''), line, column);

        return true;
    }
}