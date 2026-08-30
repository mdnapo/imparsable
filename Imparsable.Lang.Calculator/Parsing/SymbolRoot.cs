using Imparsable.Lang.Calculator.Parsing.Interfaces;

namespace Imparsable.Lang.Calculator.Parsing;

public class SymbolRoot : SymbolTable
{
    private readonly Stack<ISymbolTable> _stack = new();

    public ISymbolTable Current => _stack.Peek();
    public event Action<ISymbolTable, ISymbolTable> Pushed = delegate { };
    public event Action<ISymbolTable, ISymbolTable> Popped = delegate { };

    public SymbolRoot() => _stack.Push(this);

    public void Push(ISymbolTable child)
    {
        var parent = _stack.Peek();
        _stack.Push(child);
        Pushed.Invoke(parent, child);
    }

    public void Pop()
    {
        var child = _stack.Pop();
        var parent = _stack.Peek();
        Popped.Invoke(parent, child);
    }
}