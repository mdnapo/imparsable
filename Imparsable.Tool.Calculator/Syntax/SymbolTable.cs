namespace Imparsable.Tool.Calculator.Syntax;

public class SymbolTable
{
    public SymbolTable? Parent { get; set; }
    public List<ISymbol> Symbols { get; } = [];

    public ISymbol? Lookup(string symbol) => Symbols.Find(s => s.Symbol == symbol);
    public ISymbol? RecursiveLookup(string symbol) => Symbols.Find(s => s.Symbol == symbol) ?? Parent?.Lookup(symbol);

    public ISymbol RequireRecursiveLookup(string symbol) =>
        RecursiveLookup(symbol) ?? throw new InvalidOperationException($"Symbol '{symbol}' could not be found.");

    public int StackDepth => Parent?.StackDepth ?? 0 + Symbols.Count;

    public int Offset(string symbol)
    {
        if (Lookup(symbol) is { } lookup)
            return Parent?.StackDepth ?? 0 + Symbols.IndexOf(lookup);

        return Parent?.Offset(symbol) ?? throw new InvalidOperationException($"Symbol '{symbol}' could not be found.");
    }
}