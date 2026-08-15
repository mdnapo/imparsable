using Imparsable.Parsing.Interfaces;

namespace Imparsable.Parsing;

public partial class Lexer<TToken>
{
    public readonly record struct Token(TToken Type, int Offset, int Length, int Line, int Column) : ISourceMarker;
}