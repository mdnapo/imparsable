using Imparsable.Parsing;
using Imparsable.Tool.Calculator.Syntax;

namespace Imparsable.Tool.Calculator;

public class TypeResolver(SyntaxTree tree) : ISyntaxVisitor<SystemType>
{
    public SymbolTable Symbols { get; } = tree.SymbolTable;
    public DiagnosticsProvider Diagnostics { get; } = tree.Diagnostics;

    public static void Execute(SyntaxTree tree) => new TypeResolver(tree).Execute();

    public void Execute()
    {
        foreach (var node in tree.Roots)
            node.Accept(this);
    }

    public SystemType Visit(BinaryExpression node)
    {
        var left = node.LeftOperand.Accept(this);
        var right = node.RightOperand.Accept(this);
        SystemType type;
        string text;

        switch (node.Operator.Type)
        {
            case Token.PLUS:
            case Token.MINUS:
            case Token.STAR:
            case Token.SLASH:
            case Token.LOWER_EQUAL:
            case Token.GREATER_EQUAL:
                if (left is not SystemType.NUMBER && right is not SystemType.NUMBER)
                {
                    text = tree.Source.GetText(node.Operator.Offset, node.Operator.Length);
                    Diagnostics.Error(
                        node.Operator,
                        $"Operator '{text}' is not valid for types '{left}' and '{right}'."
                    );
                    type = SystemType.UNKNOWN;
                }
                else
                {
                    type = SystemType.NUMBER;
                }

                break;

            case Token.EQUAL_EQUAL:
            case Token.BANG_EQUAL:
                if (left is SystemType.STRING && right is SystemType.STRING)
                {
                    type = SystemType.STRING;
                }
                else if (left is SystemType.NUMBER && right is SystemType.NUMBER)
                {
                    type = SystemType.NUMBER;
                }
                else if (left is SystemType.BOOL && right is SystemType.BOOL)
                {
                    type = SystemType.BOOL;
                }
                else
                {
                    text = tree.Source.GetText(node.Operator.Offset, node.Operator.Length);
                    Diagnostics.Error(
                        node.Operator,
                        $"Operator '{text}' is not valid for types '{left}' and '{right}'."
                    );
                    type = SystemType.UNKNOWN;
                }

                break;

            case Token.DOT:
                type = SystemType.STRING;
                break;

            default:
                Diagnostics.Error(node.Operator, $"SystemType.UNKNOWN binary operator '{node.Operator.Type}'.");
                type = SystemType.UNKNOWN;
                break;
        }

        tree.Types[node] = type;
        return type;
    }

    public SystemType Visit(ConstStatement node)
    {
        var type = node.Initializer.Accept(this);
        tree.Types[node] = type;
        return SystemType.NONE;
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
        return SystemType.NONE;
    }

    public SystemType Visit(WhileStatement node)
    {
        node.Condition.Accept(this);
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
        SystemType type;

        switch (node.Operator.Type)
        {
            case Token.PLUS_EQUAL:
            case Token.MINUS_EQUAL:
            case Token.STAR_EQUAL:
            case Token.SLASH_EQUAL:
                if (target is SystemType.NUMBER && value is SystemType.NUMBER)
                {
                    type = SystemType.NUMBER;
                }
                else
                {
                    var text = tree.Source.GetText(node.Operator.Offset, node.Operator.Length);
                    Diagnostics.Error(
                        node.Operator,
                        $"Operator '{text}' is not valid for types '{target}' and '{value}'."
                    );
                    type = SystemType.UNKNOWN;
                }

                break;

            default:
                Diagnostics.Error(node.Operator, $"SystemType.UNKNOWN binary operator '{node.Operator.Type}'.");
                type = SystemType.UNKNOWN;
                break;
        }

        tree.Types[node] = type;

        return type;
    }

    public SystemType Visit(BlockStatement node)
    {
        foreach (var statement in node.Body)
            statement.Accept(this);
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
        node.Initializer?.Accept(this);

        var conditionType = node.Condition.Accept(this);
        if (conditionType != SystemType.BOOL)
            Diagnostics.Error(node.Condition.Token, $"Condition expression must be of type '{SystemType.BOOL}'.");

        node.Increment?.Accept(this);
        node.Body.Accept(this);
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