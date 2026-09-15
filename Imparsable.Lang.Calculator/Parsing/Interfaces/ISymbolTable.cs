using Imparsable.Toolchain.Parsing.Interfaces;

namespace Imparsable.Lang.Calculator.Parsing.Interfaces;

public interface ISymbolTable : IEnumerable<ISymbol>
{
    ISymbolTable? Parent { get; set; }
    int StackDepth { get; }

    void Add(ISymbol symbol);
    ISymbol? Lookup(ISourceMarker symbol);
    ISymbol? RecursiveLookup(ISourceMarker symbol);
    int Offset(ISourceMarker symbol);
}