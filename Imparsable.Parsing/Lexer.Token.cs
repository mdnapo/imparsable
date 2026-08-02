namespace Imparsable.Parsing;

public partial class Lexer<TToken>
{
    public readonly struct Token(string file, TToken type, string lexeme, int line, int column) : ISourceMarker
    {
        public string File { get; } = file;
        public TToken Type { get; } = type;
        public string Lexeme { get; } = lexeme;
        public int Line { get; } = line;
        public int Column { get; } = column;
    }
}