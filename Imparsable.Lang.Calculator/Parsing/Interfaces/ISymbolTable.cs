namespace Imparsable.Lang.Calculator.Parsing.Interfaces;

public interface ISymbolTable : IEnumerable<ISymbol>
{
    ISymbolTable? Parent { get; set; }
    int StackDepth { get; }

    void Add(ISymbol symbol);
    ISymbol? Lookup(string symbol);
    ISymbol? RecursiveLookup(string symbol);
    int Offset(string symbol);
}