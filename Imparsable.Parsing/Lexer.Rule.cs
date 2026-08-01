namespace Imparsable.Parsing;

public abstract partial class Lexer<TToken>
{
    public abstract partial class Rule
    {
        public abstract bool Match(Context context);

        private static bool IsAlpha(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';

        private static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

        private static bool IsDigit(char c) => c is >= '0' and <= '9';

        private static void ProcessDigits(Source src)
        {
            while (IsDigit(src.Peek()))
                src.Advance();
        }
    }
}