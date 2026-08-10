namespace Imparsable.Parsing;

public partial class Lexer<TToken>
{
    public readonly record struct Token(string File, TToken Type, string Lexeme, int Line, int Column) : ISourceMarker
    {
        public string File { get; } = File;
        public TToken Type { get; } = Type;
        public string Lexeme { get; } = Lexeme;
        public int Line { get; } = Line;
        public int Column { get; } = Column;
    }
}