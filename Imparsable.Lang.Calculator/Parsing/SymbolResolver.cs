using Imparsable.Lang.Calculator.Extensions;
using Imparsable.Tools.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public class SymbolResolver(SyntaxTree tree, DiagnosticsProvider diagnostics) : ISyntaxVisitor
{
    private readonly Stack<SymbolTable> _symbolTables = new([tree.SymbolTable]);
    private readonly Dictionary<ISymbol, bool> _definitions = new();
    private SymbolTable CurrentSymbolTable => _symbolTables.Peek();
    private DiagnosticsProvider Diagnostics { get; } = diagnostics;

    public static void Execute(SyntaxTree tree, DiagnosticsProvider diagnostics)
    {
        var resolver = new SymbolResolver(tree, diagnostics);
        foreach (var node in tree.Roots)
            node.Accept(resolver);
    }

    private void BeginScope(SymbolTable symbolTable)
    {
        symbolTable.Parent = CurrentSymbolTable;
        _symbolTables.Push(symbolTable);
    }

    private void EndScope() => _symbolTables.Pop();

    private void Declare(ISymbol symbol)
    {
        var syntax = symbol.As<ISyntax>();

        if (CurrentSymbolTable.Lookup(symbol.Symbol) is { } lookup)
        {
            Diagnostics.Error(lookup.As<ISyntax>().Token, $"Duplicate declaration of '{lookup.Symbol}'.");
            Diagnostics.Error(symbol.As<ISyntax>().Token, $"Duplicate declaration of '{lookup.Symbol}'.");
        }
        else if (CurrentSymbolTable.RecursiveLookup(symbol.Symbol) is not null)
        {
            Diagnostics.Warning(syntax.Token, $"Symbol '{symbol.Symbol}' hides outer declaration.");
        }

        CurrentSymbolTable.Symbols.Add(symbol);
        _definitions.Add(symbol, false);
    }

    private void Define(ISymbol symbol) => _definitions[symbol] = true;

    public void Visit(BinaryExpression node)
    {
        node.LeftOperand.Accept(this);
        node.RightOperand.Accept(this);
    }

    public void Visit(ConstStatement node)
    {
        Declare(node);
        node.Initializer.Accept(this);
        Define(node);
    }

    public void Visit(ExpressionStatement node) => node.Expression.Accept(this);

    public void Visit(GroupingExpression node) => node.Expression.Accept(this);

    public void Visit(IdentifierExpression node)
    {
        if (CurrentSymbolTable.RecursiveLookup(node.Symbol) is not { } symbol)
            Diagnostics.Error(node.Token, $"Variable '{node.Symbol}' has not been declared.");

        else if (!_definitions.TryGetValue(symbol, out var defined) || !defined)
            Diagnostics.Error(node.Token, $"Variable '{node.Symbol}' has not been defined.");
    }

    public void Visit(NumericLiteralExpression node) { }

    public void Visit(PrintStatement node) => node.Expression.Accept(this);

    public void Visit(StringLiteralExpression node) { }

    public void Visit(UnaryExpression node) => node.Operand.Accept(this);

    public void Visit(VarStatement node)
    {
        Declare(node);
        if (node.Initializer is not null)
        {
            node.Initializer.Accept(this);
            Define(node);
        }
    }

    public void Visit(IfStatement node)
    {
        node.Condition.Accept(this);
        node.Body.Accept(this);
        node.ElseIf?.Accept(this);
        node.Else?.Accept(this);
    }

    public void Visit(BlockStatement node)
    {
        BeginScope(node.SymbolTable);

        foreach (var statement in node.Body)
            statement.Accept(this);

        EndScope();
    }

    public void Visit(ElseIfStatement node)
    {
        node.Condition.Accept(this);
        node.Body.Accept(this);
        node.Next?.Accept(this);
    }

    public void Visit(ForStatement node)
    {
        BeginScope(node.SymbolTable);

        node.Initializer?.Accept(this);
        node.Condition.Accept(this);
        node.Increment?.Accept(this);
        node.Body.Accept(this);

        EndScope();
    }

    public void Visit(WhileStatement node)
    {
        node.Condition.Accept(this);
        node.Body.Accept(this);
    }

    public void Visit(BoolLiteralExpression node) { }

    public void Visit(AssignmentExpression node)
    {
        if (node.Target is not IdentifierExpression target)
        {
            Diagnostics.Error(node.Target.Token, "Invalid assignment target.");
            node.Value.Accept(this);
        }
        else if (CurrentSymbolTable.RecursiveLookup(target.Symbol) is ConstStatement)
        {
            Diagnostics.Error(node.Token, "Cannot reassign const.");
            node.Value.Accept(this);
        }
        else if (CurrentSymbolTable.RecursiveLookup(target.Symbol) is VarStatement var)
        {
            node.Value.Accept(this);
            Define(var);
        }
    }
}