using Imparsable.Tools.Parsing;

namespace Imparsable.Lang.Calculator.Parsing;

public partial class TypeResolver(SyntaxTree tree, DiagnosticsProvider diagnostics) : ISyntaxVisitor<SystemType>
{
    private const string IncompatibleOperandsErrorMessage = "Invalid operation '{0}' for types '{1}' and '{2}'.";

    private readonly Stack<SymbolTable> _symbolTables = new([tree.SymbolTable]);
    private SymbolTable Symbols => _symbolTables.Peek();
    public DiagnosticsProvider Diagnostics { get; } = diagnostics;

    public static void Execute(SyntaxTree tree, DiagnosticsProvider diagnostics) =>
        new TypeResolver(tree, diagnostics).Execute();

    public void Execute()
    {
        foreach (var node in tree.Roots)
            node.Accept(this);
    }

    private void BeginScope(SymbolTable symbolTable) => _symbolTables.Push(symbolTable);
    private void EndScope() => _symbolTables.Pop();

    public SystemType Visit(BinaryExpression node)
    {
        var leftType = node.LeftOperand.Accept(this);
        var rightType = node.RightOperand.Accept(this);
        var resultType = BinaryOperation.Resolve(node.Operator.Type, leftType, rightType);

        if (resultType is SystemType.UNKNOWN)
        {
            var op = tree.Source.GetText(node.Operator.Offset, node.Operator.Length);
            Diagnostics.Error(node.Operator, string.Format(IncompatibleOperandsErrorMessage, op, leftType, rightType));
        }

        tree.Types[node] = resultType;

        return resultType;
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
        if (node.Target is not IdentifierExpression)
            Diagnostics.Error(node.Target.Token, "Invalid assignment target.");

        var targetType = node.Target.Accept(this);
        var valueType = node.Value.Accept(this);
        var resultType = AssignmentOperation.Resolve(node.Operator.Type, targetType, valueType);

        if (resultType is SystemType.UNKNOWN)
        {
            var op = tree.Source.GetText(node.Operator.Offset, node.Operator.Length);
            Diagnostics.Error(node.Operator, string.Format(IncompatibleOperandsErrorMessage, op, targetType, valueType));
        }

        tree.Types[node] = resultType;

        return resultType;
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