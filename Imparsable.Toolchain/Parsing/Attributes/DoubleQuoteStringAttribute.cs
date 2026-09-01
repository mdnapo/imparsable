namespace Imparsable.Toolchain.Parsing.Attributes;

public sealed class DoubleQuoteStringAttribute<TToken> : LexerRuleAttribute<TToken> where TToken : Enum
{
    public override int Priority => 40;

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
            context.Halt("Unterminated string.");
        }

        // Include the closing quotation mark.
        src.Advance();

        var range = src.Extract();
        context.AddToken(Type, range.Offset, range.Length, line, column);

        return true;
    }
}