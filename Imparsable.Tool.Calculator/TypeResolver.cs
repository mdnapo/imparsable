using Imparsable.Parsing;
using Imparsable.Parsing.Interfaces;
using Imparsable.Tool.Calculator.Syntax;

namespace Imparsable.Tool.Calculator;

public class TypeResolver(SyntaxTree tree) : ISyntaxVisitor<string>
{
    private const string None = "none";
    private const string Unknown = "unknown";
    private const string Number = "number";
    private const string String = "string";

    public SymbolTable Symbols { get; } = tree.SymbolTable;
    public DiagnosticsProvider Diagnostics { get; } = tree.Diagnostics;

    private readonly Dictionary<ISymbol, string> _symbols = new();

    public static void Execute(SyntaxTree tree) => new TypeResolver(tree).Execute();

    public void Execute()
    {
        foreach (var node in tree.Roots)
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
                {
                    var text = tree.Source.GetText(node.Op.Offset, node.Op.Length);
                    Diagnostics.Error(node.Op, $"Operation '{left} {text} {right}' is invalid.");
                    return Unknown;
                }

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

    public string Visit(IdentifierExpression node) =>
        Symbols.RecursiveLookup(node.Symbol) is { } symbol && _symbols.TryGetValue(symbol, out var value)
            ? value
            : Unknown;

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