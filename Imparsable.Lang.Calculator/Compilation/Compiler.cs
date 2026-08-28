using System.Buffers.Binary;
using System.Text;
using Imparsable.Lang.Calculator.Extensions;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Tools.Compilation;

namespace Imparsable.Lang.Calculator.Compilation;

public partial class Compiler(SyntaxTree tree) : Compiler<OpCode>, ISyntaxVisitor
{
    private static readonly NumericLiteralExpression ZeroValue = new() { Token = default, Value = 0 };

    private readonly Stack<SymbolTable> _symbolTables = new([tree.SymbolTable]);
    private readonly Stack<List<int>> _elseJumps = [];

    private SymbolTable CurrentSymbolTable => _symbolTables.Peek();

    public static Chunk Execute(SyntaxTree tree)
    {
        var compiler = new Compiler(tree);

        foreach (var node in tree.Roots)
            node.Accept(compiler);

        return compiler.Build();
    }

    private void BeginScope(SymbolTable symbolTable) => _symbolTables.Push(symbolTable);

    private void EndScope()
    {
        foreach (var _ in CurrentSymbolTable.Symbols)
            EmitOpCode(OpCode.POP);

        _symbolTables.Pop();
    }

    private void EmitToString(StringConversion conversion)
    {
        EmitOpCode(OpCode.TO_STRING);
        EmitByte(conversion);
    }

    private void EmitToStringConversion(SystemType type, BinaryOperation operation)
    {
        if (type != operation.ConversionTarget)
            return;

        EmitToString(operation.Conversion ?? throw new InvalidOperationException($"Missing conversion for '{operation}'."));
    }

    public void Visit(BinaryExpression node)
    {
        var leftType = tree.Types[node.LeftOperand];
        var rightType = tree.Types[node.RightOperand];
        var operation = BinaryOperation.Resolve(node.Operator.Type, leftType, rightType);

        node.LeftOperand.Accept(this);
        EmitToStringConversion(leftType, operation);

        node.RightOperand.Accept(this);
        EmitToStringConversion(rightType, operation);

        EmitOpCode(operation.OpCode);
        if (operation.Equality is { } equality)
            EmitByte(equality);
    }

    public void Visit(ConstStatement node)
    {
        node.Initializer.Accept(this);
        EmitOpCode(OpCode.SET_LOCAL);
        var offset = CurrentSymbolTable.Offset(node.Symbol);
        EmitInt32(offset);
    }

    public void Visit(ExpressionStatement node)
    {
        node.Expression.Accept(this);
        EmitOpCode(OpCode.POP);
    }

    public void Visit(GroupingExpression node) => node.Expression.Accept(this);

    public void Visit(IdentifierExpression node)
    {
        EmitOpCode(OpCode.GET_LOCAL);
        var offset = CurrentSymbolTable.Offset(node.Symbol);
        EmitInt32(offset);
    }

    public void Visit(NumericLiteralExpression node)
    {
        using var buffer = ByteBuffer.Acquire(sizeof(double));
        var span = buffer.Span;

        BinaryPrimitives.WriteDoubleLittleEndian(span, node.Value);
        EmitOpCode(OpCode.NUM_CONST);
        var index = AddConstant(span);
        EmitInt32(index);
    }

    private void PrintOperand(ISyntax node)
    {
        node.Accept(this);

        var type = tree.Types[node];

        if (type == SystemType.STRING) return;

        var conversion = type switch
        {
            SystemType.BOOL => StringConversion.BOOL,
            SystemType.NUMBER => StringConversion.NUMBER,
            _ => throw new InvalidOperationException()
        };

        EmitToString(conversion);
    }

    public void Visit(PrintStatement node)
    {
        PrintOperand(node.Expression);
        EmitOpCode(OpCode.PRINT);
    }

    public void Visit(StringLiteralExpression node)
    {
        var @string = node.Value.Trim('\'', '"');
        var length = @string.Length;
        var size = sizeof(int) + length;
        using var buffer = ByteBuffer.Acquire(size);
        var span = buffer.Span;

        BinaryPrimitives.WriteInt32LittleEndian(span[..sizeof(int)], length);
        Encoding.UTF8.GetBytes(@string, span[sizeof(int)..]);

        EmitOpCode(OpCode.STRING_CONST);
        var index = AddConstant(span);
        EmitInt32(index);
    }

    public void Visit(UnaryExpression node)
    {
        node.Operand.Accept(this);
        var text = tree.Source.GetText(node.Token.Offset, node.Token.Length);

        var op = node.Op.Type switch
        {
            Token.MINUS => OpCode.NEGATE_NUM,
            Token.BANG => OpCode.NEGATE_BOOL,
            _ => throw new InvalidOperationException($"Unsupported operator '{text}' for binary expression.")
        };

        EmitOpCode(op);
    }

    public void Visit(VarStatement node)
    {
        if (node.Initializer is not null)
        {
            node.Initializer.Accept(this);
        }
        else
        {
            ZeroValue.Accept(this);
        }

        EmitOpCode(OpCode.SET_LOCAL);
        var offset = CurrentSymbolTable.Offset(node.Symbol);
        EmitInt32(offset);
    }

    public void Visit(IfStatement node)
    {
        node.Condition.Accept(this);

        var thenJump = EmitJump(OpCode.JMP_FALSE);

        EmitOpCode(OpCode.POP);

        node.Body.Accept(this);

        _elseJumps.Push([EmitJump(OpCode.JMP)]);

        PatchJump(thenJump);

        node.ElseIf?.Accept(this);
        node.Else?.Accept(this);

        foreach (var jump in _elseJumps.Peek())
            PatchJump(jump);

        _elseJumps.Pop();
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
        EmitOpCode(OpCode.POP);

        node.Condition.Accept(this);

        var thenJump = EmitJump(OpCode.JMP_FALSE);

        EmitOpCode(OpCode.POP);

        node.Body.Accept(this);

        _elseJumps.Peek().Add(EmitJump(OpCode.JMP));

        PatchJump(thenJump);

        node.Next?.Accept(this);
    }

    public void Visit(ForStatement node)
    {
        BeginScope(node.SymbolTable);

        node.Initializer?.Accept(this);

        var loopStart = Code.Count;

        node.Condition.Accept(this);

        var exitJump = EmitJump(OpCode.JMP_FALSE);

        EmitOpCode(OpCode.POP);

        node.Body.Accept(this);

        if (node.Increment is not null)
        {
            node.Increment.Accept(this);
            EmitOpCode(OpCode.POP);
        }

        EmitLoop(OpCode.JMP, loopStart);

        PatchJump(exitJump);

        EmitOpCode(OpCode.POP);

        EndScope();
    }

    public void Visit(WhileStatement node)
    {
        var loopStart = Code.Count;

        node.Condition.Accept(this);

        var exitJump = EmitJump(OpCode.JMP_FALSE);

        EmitOpCode(OpCode.POP);

        node.Body.Accept(this);

        EmitLoop(OpCode.JMP, loopStart);

        PatchJump(exitJump);

        EmitOpCode(OpCode.POP);
    }

    public void Visit(BoolLiteralExpression node)
    {
        EmitOpCode(OpCode.BOOL_CONST);
        EmitByte(node.Value ? BoolValue.TRUE : BoolValue.FALSE);
    }

    public void Visit(AssignmentExpression node)
    {
        var identifier = node.Target.As<IdentifierExpression>();
        var assignment = AssignmentOperation.Resolve(node.Operator.Type);
        var offset = CurrentSymbolTable.Offset(identifier.Symbol);

        if (assignment.BinaryOperator is null)
        {
            node.Value.Accept(this);
        }
        else
        {
            var targetType = tree.Types[node.Target];
            var valueType = tree.Types[node.Value];
            var operation = BinaryOperation.Resolve(assignment.BinaryOperator.Value, targetType, valueType);

            EmitOpCode(OpCode.GET_LOCAL);
            EmitInt32(offset);

            node.Value.Accept(this);
            EmitToStringConversion(valueType, operation);
            EmitOpCode(operation.OpCode);
        }

        EmitOpCode(OpCode.SET_LOCAL);
        EmitInt32(offset);
    }
}