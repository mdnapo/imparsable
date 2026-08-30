using System.Collections;
using Imparsable.Lang.Calculator.Parsing.Interfaces;

namespace Imparsable.Lang.Calculator.Parsing;

public class SymbolTable : ISymbolTable
{
    protected List<ISymbol> Symbols { get; } = [];
    public ISymbolTable? Parent { get; set; }
    public int StackDepth => Parent?.StackDepth ?? 0 + Symbols.Count;

    public virtual void Add(ISymbol symbol) => Symbols.Add(symbol);
    public virtual ISymbol? Lookup(string symbol) => Symbols.Find(s => s.Symbol == symbol);
    public virtual ISymbol? RecursiveLookup(string symbol) => Symbols.Find(s => s.Symbol == symbol) ?? Parent?.RecursiveLookup(symbol);

    public virtual int Offset(string symbol)
    {
        if (Lookup(symbol) is { } lookup)
            return Parent?.StackDepth ?? 0 + Symbols.IndexOf(lookup);

        return Parent?.Offset(symbol) ?? throw new InvalidOperationException($"Symbol '{symbol}' could not be found.");
    }

    public virtual IEnumerator<ISymbol> GetEnumerator() => Symbols.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}