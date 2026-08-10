using Imparsable.Parsing;
using Imparsable.Tool.Calculator.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Imparsable.Tool.Calculator;

public partial class Runtime(IServiceProvider services) : ISyntaxVisitor
{
    private readonly Stack<object> _stack = [];
    private Stack<Scope> Scopes { get; } = new([new Scope()]);

    public event Action<string> StdOut = delegate { };

    public void Execute(string file, string source)
    {
        using var scope = services.CreateScope();
        scope.ServiceProvider
            .GetRequiredService<Lexer<Token>.ContextProvider>()
            .Initialize(file, source);

        var parser = scope.ServiceProvider.GetRequiredService<Parser<Token, ISyntax>>();
        var syntax = parser.Execute<Statement.Production, Statement.Synchronizer>();
        scope.ServiceProvider.GetRequiredService<SymbolResolver>().Execute(syntax);

        var diagnostics = scope.ServiceProvider.GetRequiredService<DiagnosticsProvider>();

        foreach (var diagnostic in diagnostics)
            StdOut(diagnostic.Report);

        if (!diagnostics.IsHealthy) return;

        foreach (var node in syntax)
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
                Scopes.Peek()[identifier!.Token.Lexeme] = right;
                break;

            default:
                throw new InvalidOperationException($"Unknown binary operator '{node.Op.Type}'.");
        }
    }

    public void Visit(ConstStatement node)
    {
        node.Initializer.Accept(this);
        Scopes.Peek().Declare(node.Identifier.Lexeme, _stack.Pop());
    }

    public void Visit(ExpressionStatement node) => node.Expr.Accept(this);

    public void Visit(GroupingExpression node) => node.Expression.Accept(this);

    public void Visit(IdentifierExpression node)
    {
        var value = Scopes.Peek()[node.Token.Lexeme];
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

        Scopes.Peek().Declare(node.Identifier.Lexeme, value!);
    }
}