namespace Imparsable.Parsing;

public partial class Lexer<TToken>
{
    public readonly record struct Token(TToken Type, string Lexeme, int Line, int Column) : ISourceMarker
    {
        public TToken Type { get; } = Type;
        public string Lexeme { get; } = Lexeme;
        public int Line { get; } = Line;
        public int Column { get; } = Column;
    }
}