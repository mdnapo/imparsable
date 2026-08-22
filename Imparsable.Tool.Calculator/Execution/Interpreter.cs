using Imparsable.Tool.Calculator.Syntax;

namespace Imparsable.Tool.Calculator.Execution;

public partial class Interpreter : ISyntaxVisitor, IDisposable
{
    private readonly Stack<object> _stack = [];
    private Stack<Scope> Scopes { get; } = new([new Scope()]);

    public event Action<string> Out = delegate { };

    public void Execute(SyntaxTree tree)
    {
        foreach (var diagnostic in tree.Diagnostics)
            Out(diagnostic.Report);

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

        switch (node.Operator.Type)
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

            case Token.EQUAL:
                var identifier = left as IdentifierExpression;
                Scopes.Peek()[identifier!.Symbol] = right;
                break;

            default:
                throw new InvalidOperationException($"Unknown binary operator '{node.Operator.Type}'.");
        }
    }

    public void Visit(ConstStatement node)
    {
        node.Initializer.Accept(this);
        Scopes.Peek().Declare(node.Symbol, _stack.Pop());
    }

    public void Visit(ExpressionStatement node) => node.Expression.Accept(this);

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
        Out(_stack.Pop().ToString()!);
    }

    public void Visit(StringLiteralExpression node) => _stack.Push(node.Value.Trim('"', '\''));

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

    public void Dispose()
    {
        foreach (var @delegate in Out.GetInvocationList())
            Out -= @delegate as Action<string>;
    }
}