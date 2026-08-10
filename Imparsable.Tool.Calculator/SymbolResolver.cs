using Imparsable.Parsing;
using Imparsable.Tool.Calculator.Syntax;

namespace Imparsable.Tool.Calculator;

public class SymbolResolver(DiagnosticsProvider diagnostics, SymbolTable symbols) : ISyntaxVisitor
{
    private string? _initializer;

    public void Execute(List<ISyntax> syntax)
    {
        foreach (var node in syntax)
            node.Accept(this);
    }

    private void Declare(ISymbol symbol)
    {
        if (symbol is not ISyntax syntax)
            throw new InvalidCastException($"Cannot cast {symbol.GetType().Name} to {typeof(ISyntax)}");

        if (symbols.Lookup(symbol.Symbol) is not null)
        {
            diagnostics.Error(syntax.Token, $"Redeclaration of symbol '{symbol.Symbol}' is not allowed.");
        }
        else if (symbols.RecursiveLookup(symbol.Symbol) is not null)
        {
            diagnostics.Warning(syntax.Token, $"Symbol '{symbol.Symbol}' hides outer declaration.");
        }

        symbols.Symbols.Add(symbol);
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

    public void Visit(ExpressionStatement node) => node.Expr.Accept(this);

    public void Visit(GroupingExpression node) => node.Expression.Accept(this);

    public void Visit(IdentifierExpression node)
    {
        if (_initializer == node.Token.Lexeme)
            diagnostics.Error(node.Token, $"Cannot use '{node.Token.Lexeme}' in it's own initializer.");

        if (symbols.RecursiveLookup(node.Token.Lexeme) is null)
            diagnostics.Error(node.Token, $"Variable '{node.Token.Lexeme}' has not been declared.");
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
}