using Imparsable.Parsing;
using Imparsable.Tool.Calculator.Syntax;

namespace Imparsable.Tool.Calculator;

public class OldSymbolResolver(SyntaxTree tree) : ISyntaxVisitor
{
    private string? _initializer;
    public SymbolTable Symbols { get; } = tree.SymbolTable;
    public DiagnosticsProvider Diagnostics { get; } = tree.Diagnostics;

    public static void Execute(SyntaxTree tree) => new OldSymbolResolver(tree).Execute();

    public void Execute()
    {
        foreach (var node in tree.Roots)
            node.Accept(this);
    }

    private void Declare(ISymbol symbol)
    {
        if (symbol is not ISyntax syntax)
            throw new InvalidCastException($"Cannot cast {symbol.GetType().Name} to {typeof(ISyntax)}");

        if (Symbols.Lookup(symbol.Symbol) is not null)
        {
            Diagnostics.Error(syntax.Token, $"Redeclaration of symbol '{symbol.Symbol}' is not allowed.");
        }
        else if (Symbols.RecursiveLookup(symbol.Symbol) is not null)
        {
            Diagnostics.Warning(syntax.Token, $"Symbol '{symbol.Symbol}' hides outer declaration.");
        }

        Symbols.Symbols.Add(symbol);
    }

    public void Visit(BinaryExpression node)
    {
        node.LeftOperand.Accept(this);
        node.RightOperand.Accept(this);
    }

    public void Visit(ConstStatement node)
    {
        _initializer = node.Symbol;
        Declare(node);
        node.Initializer.Accept(this);
        _initializer = null;
    }

    public void Visit(ExpressionStatement node) => node.Expression.Accept(this);

    public void Visit(GroupingExpression node) => node.Expression.Accept(this);

    public void Visit(IdentifierExpression node)
    {
        if (_initializer == node.Symbol)
            Diagnostics.Error(node.Token, $"Cannot use '{node.Symbol}' in it's own initializer.");

        if (Symbols.RecursiveLookup(node.Symbol) is null)
            Diagnostics.Error(node.Token, $"Variable '{node.Symbol}' has not been declared.");
    }

    public void Visit(NumericLiteralExpression node) { }

    public void Visit(PrintStatement node) => node.Expression.Accept(this);

    public void Visit(StringLiteralExpression node) { }

    public void Visit(UnaryExpression node) => node.Operand.Accept(this);

    public void Visit(VarStatement node)
    {
        _initializer = node.Symbol;
        Declare(node);
        node.Initializer?.Accept(this);
        _initializer = null;
    }

    public void Visit(WhileStatement node)
    {
        throw new NotImplementedException();
    }

    public void Visit(BoolLiteralExpression node)
    {
        throw new NotImplementedException();
    }

    public void Visit(AssignmentExpression node)
    {
        throw new NotImplementedException();
    }

    public void Visit(BlockStatement node)
    {
        throw new NotImplementedException();
    }

    public void Visit(IfStatement node)
    {
        throw new NotImplementedException();
    }

    public void Visit(ForStatement node)
    {
        throw new NotImplementedException();
    }

    public void Visit(ElseIfStatement node)
    {
        throw new NotImplementedException();
    }
}