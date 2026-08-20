using Imparsable.Parsing;
using Imparsable.Tool.Calculator.Syntax;

namespace Imparsable.Tool.Calculator;

public class TypeResolver(SyntaxTree tree) : ISyntaxVisitor<string>
{
    public const string None = "none";
    public const string Unknown = "unknown";
    public const string BoolType = "bool";
    public const string NumberType = "number";
    public const string StringType = "string";

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
        string type, text;

        switch (node.Operator.Type)
        {
            case Token.PLUS:
            case Token.MINUS:
            case Token.STAR:
            case Token.SLASH:
            case Token.LOWER_EQUAL:
            case Token.GREATER_EQUAL:
                if (left is not NumberType && right is not NumberType)
                {
                    text = tree.Source.GetText(node.Operator.Offset, node.Operator.Length);
                    Diagnostics.Error(
                        node.Operator,
                        $"Operator '{text}' is not valid for types '{left}' and '{right}'."
                    );
                    type = Unknown;
                }
                else
                {
                    type = NumberType;
                }

                break;

            case Token.EQUAL_EQUAL:
            case Token.BANG_EQUAL:
                if (left is StringType && right is StringType)
                {
                    type = StringType;
                }
                else if (left is NumberType && right is NumberType)
                {
                    type = NumberType;
                }
                else if (left is BoolType && right is BoolType)
                {
                    type = BoolType;
                }
                else
                {
                    text = tree.Source.GetText(node.Operator.Offset, node.Operator.Length);
                    Diagnostics.Error(
                        node.Operator,
                        $"Operator '{text}' is not valid for types '{left}' and '{right}'."
                    );
                    type = Unknown;
                }
                break;

            case Token.DOT:
                type = StringType;
                break;

            default:
                Diagnostics.Error(node.Operator, $"Unknown binary operator '{node.Operator.Type}'.");
                type = Unknown;
                break;
        }

        tree.TypeMap[node] = type;
        return type;
    }

    public string Visit(ConstStatement node)
    {
        var type = node.Initializer.Accept(this);
        _symbols[node] = type;
        return None;
    }

    public string Visit(ExpressionStatement node)
    {
        node.Expression.Accept(this);
        return None;
    }

    public string Visit(GroupingExpression node)
    {
        var type = node.Expression.Accept(this);
        tree.TypeMap[node] = type;
        return type;
    }

    public string Visit(IdentifierExpression node)
    {
        var type = Symbols.RecursiveLookup(node.Symbol) is { } symbol && _symbols.TryGetValue(symbol, out var value)
            ? value
            : Unknown;

        tree.TypeMap[node] = type;

        return type;
    }

    public string Visit(NumericLiteralExpression node)
    {
        tree.TypeMap[node] = NumberType;
        return NumberType;
    }

    public string Visit(PrintStatement node)
    {
        node.Expression.Accept(this);
        return None;
    }

    public string Visit(StringLiteralExpression node)
    {
        tree.TypeMap[node] = StringType;
        return StringType;
    }

    public string Visit(UnaryExpression node)
    {
        var type = node.Operand.Accept(this);
        tree.TypeMap[node] = type;
        return type;
    }

    public string Visit(VarStatement node)
    {
        var type = node.Initializer?.Accept(this);
        _symbols[node] = type ?? Unknown;
        return None;
    }

    public string Visit(WhileStatement node)
    {
        node.Condition.Accept(this);
        node.Body.Accept(this);
        return None;
    }

    public string Visit(BoolLiteralExpression node)
    {
        tree.TypeMap[node] = BoolType;
        return BoolType;
    }

    public string Visit(AssignmentExpression node)
    {
        var target = node.Target.Accept(this);
        var value = node.Value.Accept(this);
        string type;

        switch (node.Operator.Type)
        {
            case Token.PLUS_EQUAL:
            case Token.MINUS_EQUAL:
            case Token.STAR_EQUAL:
            case Token.SLASH_EQUAL:
                if (target is NumberType && value is NumberType)
                {
                    type = NumberType;
                }
                else
                {
                    var text = tree.Source.GetText(node.Operator.Offset, node.Operator.Length);
                    Diagnostics.Error(
                        node.Operator,
                        $"Operator '{text}' is not valid for types '{target}' and '{value}'."
                    );
                    type = Unknown;
                }

                break;

            default:
                Diagnostics.Error(node.Operator, $"Unknown binary operator '{node.Operator.Type}'.");
                type = Unknown;
                break;
        }

        tree.TypeMap[node] = type;

        return type;
    }

    public string Visit(BlockStatement node)
    {
        foreach (var statement in node.Body)
            statement.Accept(this);
        return None;
    }

    public string Visit(IfStatement node)
    {
        var conditionType = node.Condition.Accept(this);
        
        if (conditionType != BoolType)
            Diagnostics.Error(node.Condition.Token, $"Condition expression must be of type '{BoolType}'.");
        
        node.Body.Accept(this);
        node.ElseIf?.Accept(this);
        node.Else?.Accept(this);
        return None;
    }

    public string Visit(ForStatement node)
    {
        node.Initializer?.Accept(this);
        
        var conditionType = node.Condition.Accept(this);
        if (conditionType != BoolType)
            Diagnostics.Error(node.Condition.Token, $"Condition expression must be of type '{BoolType}'.");
        
        node.Increment?.Accept(this);
        node.Body.Accept(this);
        return None;
    }

    public string Visit(ElseIfStatement node)
    {
        var conditionType = node.Condition.Accept(this);
        if (conditionType != BoolType)
            Diagnostics.Error(node.Condition.Token, $"Condition expression must be of type '{BoolType}'.");
        
        node.Body.Accept(this);
        node.Next?.Accept(this);
        return None;
    }
}