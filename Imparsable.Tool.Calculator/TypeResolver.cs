using Imparsable.Parsing;
using Imparsable.Parsing.Interfaces;
using Imparsable.Tool.Calculator.Syntax;

namespace Imparsable.Tool.Calculator;

public class TypeResolver(DiagnosticsProvider diagnostics, SymbolTable symbols) : ISyntaxVisitor<string>
{
    private const string None = "none";
    private const string Number = "number";
    private const string String = "string";
    
    public SymbolTable Symbols { get; } = symbols;
    public DiagnosticsProvider Diagnostics { get; } = diagnostics;

    private readonly Dictionary<ISymbol, string> _symbols = new();
    
    public static void Execute(SyntaxTree tree) =>
        new TypeResolver(tree.Diagnostics, tree.SymbolTable).Execute(tree.Roots);

    public void Execute(List<ISyntax> nodes)
    {
        foreach (var node in nodes)
            node.Accept(this);
    }

    public string Visit(BinaryExpression node)
    {
        var left = node.LeftOperand.Accept(this);
        var right = node.RightOperand.Accept(this);

        switch (node.Op.Type)
        {
            case Token.PLUS:
            case Token.MINUS:
            case Token.STAR:
            case Token.SLASH:
                if (left is not Number || right is not Number)
                    Diagnostics.Error(node.Op, $"Operator '{node.Op.Lexeme}' can only operate on numbers.");
                return Number;

            case Token.DOT:
                return String;

            default:
                throw new InvalidOperationException($"Unknown binary operator '{node.Op.Type}'.");
        }
    }

    public string Visit(ConstStatement node)
    {
        var type = node.Initializer.Accept(this);
        _symbols[node] = type;
        return None;
    }

    public string Visit(ExpressionStatement node)
    {
        node.Expr.Accept(this);
        return None;
    }

    public string Visit(GroupingExpression node) => node.Expression.Accept(this);

    public string Visit(IdentifierExpression node) => _symbols[Symbols.RequireRecursiveLookup(node.Token.Lexeme)];

    public string Visit(NumericLiteralExpression node) => Number;

    public string Visit(PrintStatement node)
    {
        node.Expression.Accept(this);
        return None;
    }

    public string Visit(StringLiteralExpression node) => String;

    public string Visit(UnaryExpression node) => node.Operand.Accept(this);

    public string Visit(VarStatement node)
    {
        var type = node.Initializer?.Accept(this) ?? None;
        _symbols[node] = type;
        return None;
    }
}