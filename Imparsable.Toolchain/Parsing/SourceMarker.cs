using Imparsable.Toolchain.Parsing.Interfaces;

namespace Imparsable.Toolchain.Parsing;

public readonly record struct SourceMarker(int Offset, int Length, int Line, int Column) : ISourceMarker;