namespace Imparsable.Parsing;

public class SymbolTable
{
    public SymbolTable? Parent { get; init; }
    public List<SymbolTable> Children { get; } = [];
    public List<ISymbol> Symbols { get; } = [];

    public SymbolTable CreateChild()
    {
        var child = new SymbolTable { Parent = this };
        Children.Add(child);
        return child;
    }

    public ISymbol? Lookup(string symbol) => Symbols.Find(s => s.Symbol == symbol);
    public ISymbol? RecursiveLookup(string symbol) => Symbols.Find(s => s.Symbol == symbol) ?? Parent?.Lookup(symbol);

    public ISymbol RequireRecursiveLookup(string symbol) =>
        RecursiveLookup(symbol) ?? throw new InvalidOperationException($"Symbol '{symbol}' could not be found.");
}