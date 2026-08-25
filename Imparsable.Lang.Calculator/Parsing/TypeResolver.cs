using Imparsable.Tools.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class TypeResolver(SyntaxTree tree) : ISyntaxVisitor<SystemType>
{
    private const string IncompatibleOperandsErrorMessage = "Invalid operation '{0}' for types '{1}' and '{2}'.";

    private readonly Stack<SymbolTable> _symbolTables = new([tree.SymbolTable]);
    private SymbolTable Symbols => _symbolTables.Peek();
    public DiagnosticsProvider Diagnostics { get; } = tree.Diagnostics;

    public static void Execute(SyntaxTree tree) => new TypeResolver(tree).Execute();

    public void Execute()
    {
        foreach (var node in tree.Roots)
            node.Accept(this);
    }

    private void BeginScope(SymbolTable symbolTable) => _symbolTables.Push(symbolTable);
    private void EndScope() => _symbolTables.Pop();

    public SystemType Visit(BinaryExpression node)
    {
        var left = node.LeftOperand.Accept(this);
        var right = node.RightOperand.Accept(this);
        var type = BinaryOperation.Resolve(node.Operator.Type, left, right);

        if (type is SystemType.UNKNOWN)
        {
            var op = tree.Source.GetText(node.Operator.Offset, node.Operator.Length);
            Diagnostics.Error(node.Operator, string.Format(IncompatibleOperandsErrorMessage, op, left, right));
        }

        tree.Types[node] = type;

        return type;
    }

    public SystemType Visit(ConstStatement node)
    {
        var type = node.Initializer.Accept(this);
        tree.Types[node] = type;
        return tree.Types[node];
    }

    public SystemType Visit(ExpressionStatement node)
    {
        node.Expression.Accept(this);
        return SystemType.NONE;
    }

    public SystemType Visit(GroupingExpression node)
    {
        var type = node.Expression.Accept(this);
        tree.Types[node] = type;
        return type;
    }

    public SystemType Visit(IdentifierExpression node)
    {
        var type = Symbols.RecursiveLookup(node.Symbol) is ISyntax symbol &&
                   tree.Types.TryGetValue(symbol, out var value)
            ? value
            : SystemType.UNKNOWN;

        tree.Types[node] = type;

        return type;
    }

    public SystemType Visit(NumericLiteralExpression node)
    {
        tree.Types[node] = SystemType.NUMBER;
        return SystemType.NUMBER;
    }

    public SystemType Visit(PrintStatement node)
    {
        node.Expression.Accept(this);
        return SystemType.NONE;
    }

    public SystemType Visit(StringLiteralExpression node)
    {
        tree.Types[node] = SystemType.STRING;
        return SystemType.STRING;
    }

    public SystemType Visit(UnaryExpression node)
    {
        var type = node.Operand.Accept(this);
        tree.Types[node] = type;
        return type;
    }

    public SystemType Visit(VarStatement node)
    {
        var type = node.Initializer?.Accept(this);
        tree.Types[node] = type ?? SystemType.UNKNOWN;
        return tree.Types[node];
    }

    public SystemType Visit(WhileStatement node)
    {
        tree.Types[node.Condition] = node.Condition.Accept(this);
        node.Body.Accept(this);
        return SystemType.NONE;
    }

    public SystemType Visit(BoolLiteralExpression node)
    {
        tree.Types[node] = SystemType.BOOL;
        return SystemType.BOOL;
    }

    public SystemType Visit(AssignmentExpression node)
    {
        var target = node.Target.Accept(this);
        var value = node.Value.Accept(this);
        var op = tree.Source.GetText(node.Operator.Offset, node.Operator.Length);
        SystemType type;

        switch (node.Operator.Type)
        {
            case Token.EQUAL:
            case Token.PLUS_EQUAL:
            case Token.MINUS_EQUAL:
            case Token.STAR_EQUAL:
            case Token.SLASH_EQUAL
                when target is SystemType.NUMBER && value is SystemType.NUMBER:
            {
                type = SystemType.NUMBER;
                break;
            }

            default:
                Diagnostics.Error(node.Operator, string.Format(IncompatibleOperandsErrorMessage, op, target, value));
                type = SystemType.UNKNOWN;
                break;
        }

        tree.Types[node] = type;

        return type;
    }

    public SystemType Visit(BlockStatement node)
    {
        BeginScope(node.SymbolTable);

        foreach (var statement in node.Body)
            statement.Accept(this);

        EndScope();

        return SystemType.NONE;
    }

    public SystemType Visit(IfStatement node)
    {
        var conditionType = node.Condition.Accept(this);

        if (conditionType != SystemType.BOOL)
            Diagnostics.Error(node.Condition.Token, $"Condition expression must be of type '{SystemType.BOOL}'.");

        node.Body.Accept(this);
        node.ElseIf?.Accept(this);
        node.Else?.Accept(this);
        return SystemType.NONE;
    }

    public SystemType Visit(ForStatement node)
    {
        BeginScope(node.SymbolTable);

        if (node.Initializer is not null)
        {
            var type = node.Initializer.Accept(this);
            tree.Types[node.Initializer] = type;
        }

        var conditionType = node.Condition.Accept(this);
        if (conditionType != SystemType.BOOL)
            Diagnostics.Error(node.Condition.Token, $"Condition expression must be of type '{SystemType.BOOL}'.");

        if (node.Increment is not null)
        {
            var type = node.Increment.Accept(this);
            tree.Types[node.Increment] = type;
        }

        node.Body.Accept(this);

        EndScope();

        return SystemType.NONE;
    }

    public SystemType Visit(ElseIfStatement node)
    {
        var conditionType = node.Condition.Accept(this);
        if (conditionType != SystemType.BOOL)
            Diagnostics.Error(node.Condition.Token, $"Condition expression must be of type '{SystemType.BOOL}'.");

        node.Body.Accept(this);
        node.Next?.Accept(this);
        return SystemType.NONE;
    }
}