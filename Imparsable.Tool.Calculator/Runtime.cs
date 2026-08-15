using Imparsable.Tool.Calculator.Syntax;

namespace Imparsable.Tool.Calculator;

public partial class Runtime : ISyntaxVisitor, IDisposable
{
    private readonly Stack<object> _stack = [];
    private Stack<Scope> Scopes { get; } = new([new Scope()]);

    public event Action<string> StdOut = delegate { };

    public void Execute(SyntaxTree tree)
    {
        foreach (var diagnostic in tree.Diagnostics)
            StdOut(diagnostic.Report);

        if (!tree.Diagnostics.IsHealthy) return;

        foreach (var node in tree.Roots)
            node.Accept(this);
    }

    public void Visit(BinaryExpression node)
    {
        node.LeftOperand.Accept(this);
        node.RightOperand.Accept(this);

        var right = _stack.Pop();
        var left = _stack.Pop();

        switch (node.Op.Type)
        {
            case Token.DOT:
                _stack.Push(string.Empty + left + right);
                break;

            case Token.PLUS:
                _stack.Push((double)left + (double)right);
                break;

            case Token.MINUS:
                _stack.Push((double)left - (double)right);
                break;

            case Token.STAR:
                _stack.Push((double)left * (double)right);
                break;

            case Token.SLASH:
                _stack.Push((double)left / (double)right);
                break;

            case Token.EQUALS:
                var identifier = left as IdentifierExpression;
                Scopes.Peek()[identifier!.Symbol] = right;
                break;

            default:
                throw new InvalidOperationException($"Unknown binary operator '{node.Op.Type}'.");
        }
    }

    public void Visit(ConstStatement node)
    {
        node.Initializer.Accept(this);
        Scopes.Peek().Declare(node.Symbol, _stack.Pop());
    }

    public void Visit(ExpressionStatement node) => node.Expr.Accept(this);

    public void Visit(GroupingExpression node) => node.Expression.Accept(this);

    public void Visit(IdentifierExpression node)
    {
        var value = Scopes.Peek()[node.Symbol];
        _stack.Push(value);
    }

    public void Visit(NumericLiteralExpression node) => _stack.Push(node.Value);

    public void Visit(PrintStatement node)
    {
        node.Expression.Accept(this);
        StdOut(_stack.Pop().ToString()!);
    }

    public void Visit(StringLiteralExpression node) => _stack.Push(node.Value);

    public void Visit(UnaryExpression node)
    {
        node.Operand.Accept(this);
        var operand = _stack.Pop();

        switch (node.Op.Type)
        {
            case Token.MINUS:
                _stack.Push(-(double)operand);
                break;

            default:
                throw new InvalidOperationException($"Unknown unary operator '{node.Op.Type}'.");
        }
    }

    public void Visit(VarStatement node)
    {
        object? value = null;

        if (node.Initializer is not null)
        {
            node.Initializer.Accept(this);
            value = _stack.Pop();
        }

        Scopes.Peek().Declare(node.Symbol, value!);
    }

    public void Dispose()
    {
        foreach (var @delegate in StdOut.GetInvocationList())
            StdOut -= @delegate as Action<string>;
    }
}