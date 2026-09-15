using System.Collections;
using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain.Parsing;
using Imparsable.Toolchain.Parsing.Interfaces;

namespace Imparsable.Lang.Calculator.Parsing;

public class SymbolTable(Source source) : ISymbolTable
{
    protected List<ISymbol> Symbols { get; } = [];
    public ISymbolTable? Parent { get; set; }
    public int StackDepth => Parent?.StackDepth ?? 0 + Symbols.Count;

    public virtual void Add(ISymbol symbol) => Symbols.Add(symbol);

    public virtual int Offset(ISourceMarker symbol)
    {
        if (Lookup(symbol) is { } lookup)
            return Parent?.StackDepth ?? 0 + Symbols.IndexOf(lookup);

        return Parent?.Offset(symbol) ?? throw new InvalidOperationException($"Symbol '{symbol}' could not be found.");
    }

    public virtual ISymbol? Lookup(ISourceMarker symbol)
    {
        var text = source.GetTextSpan(symbol);

        foreach (var value in Symbols)
            if (source.GetTextSpan(value.Symbol).SequenceEqual(text))
                return value;

        return null;
    }

    public virtual ISymbol? RecursiveLookup(ISourceMarker symbol) =>
        Lookup(symbol) ?? Parent?.RecursiveLookup(symbol);

    public virtual IEnumerator<ISymbol> GetEnumerator() => Symbols.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}