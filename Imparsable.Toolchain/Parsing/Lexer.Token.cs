using Imparsable.Toolchain.Parsing.Interfaces;

namespace Imparsable.Toolchain.Parsing;

public partial class Lexer<TToken>
{
    public readonly record struct Token(TToken Type, int Offset, int Length, int Line, int Column) : ISourceMarker;
}