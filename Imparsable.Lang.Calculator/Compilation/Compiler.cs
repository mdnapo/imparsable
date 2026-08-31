using System.Buffers.Binary;
using System.Text;
using Imparsable.Lang.Calculator.Exceptions;
using Imparsable.Lang.Calculator.Extensions;
using Imparsable.Lang.Calculator.Parsing;
using Imparsable.Lang.Calculator.Parsing.Interfaces;
using Imparsable.Toolchain;
using Imparsable.Toolchain.Compilation;
using Imparsable.Toolchain.Parsing.Interfaces;

namespace Imparsable.Lang.Calculator.Compilation;

public partial class Compiler(SyntaxTree tree, DiagnosticsProvider diagnostics) : Compiler<OpCode>, ISyntaxVisitor
{
    private static readonly NumericLiteralExpression ZeroValue = new() { Token = default, Value = 0 };
    private readonly Stack<List<int>> _elseJumps = [];
    private readonly Stack<List<int>> _breaks = [];
    private readonly Stack<List<int>> _continues = [];

    private SymbolRoot SymbolRoot => tree.SymbolRoot;
    private ISymbolTable Symbols => tree.SymbolRoot.Current;
    private DiagnosticsProvider Diagnostics { get; } = diagnostics;

    public static Chunk? Execute(SyntaxTree tree, DiagnosticsProvider diagnostics) =>
        new Compiler(tree, diagnostics).Execute();

    public Chunk? Execute()
    {
        try
        {
            tree.SymbolRoot.Popped += OnPop;

            foreach (var node in tree.Roots)
                node.Accept(this);

            return Build();
        }
        catch (HaltException)
        {
            return null;
        }
        finally
        {
            tree.SymbolRoot.Popped -= OnPop;
        }
    }

    private void OnPop(ISymbolTable parent, ISymbolTable child)
    {
        foreach (var _ in child)
            EmitOpCode(OpCode.POP);
    }

    private void EmitToString(StringConversion conversion)
    {
        EmitOpCode(OpCode.TO_STRING);
        EmitByte(conversion);
    }

    private void EmitToStringConversion(ISourceMarker marker, SystemType type, BinaryOperation operation)
    {
        if (type != operation.ConversionTarget)
            return;

        if (operation.Conversion is not { } conversion)
            throw Diagnostics.Halt<HaltException>(marker, $"Missing string conversion for type '{type}'.");

        EmitToString(conversion);
    }

    public void Visit(BinaryExpression node)
    {
        var leftType = tree.Types[node.LeftOperand];
        var rightType = tree.Types[node.RightOperand];
        var @operator = node.Operator;
        var operation = BinaryOperation.Resolve(@operator.Type, leftType, rightType);

        node.LeftOperand.Accept(this);
        EmitToStringConversion(node.LeftOperand.Token, leftType, operation);

        node.RightOperand.Accept(this);
        EmitToStringConversion(node.RightOperand.Token, rightType, operation);

        EmitOpCode(operation.OpCode);
        if (operation.Equality is { } equality)
            EmitByte(equality);
    }

    public void Visit(ConstStatement node)
    {
        node.Initializer.Accept(this);
        EmitOpCode(OpCode.SET_LOCAL);
        var offset = Symbols.Offset(node.Symbol);
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
        var offset = Symbols.Offset(node.Symbol);
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
            _ => throw Diagnostics.Halt<HaltException>(node.Token, $"Missing string conversion for type '{type}'.")
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
            _ => throw Diagnostics.Halt<HaltException>(node.Token, $"Unsupported operator '{text}' for binary expression.")
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
        var offset = Symbols.Offset(node.Symbol);
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
        SymbolRoot.Push(node);

        foreach (var statement in node.Body)
            statement.Accept(this);

        SymbolRoot.Pop();
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
        SymbolRoot.Push(node);
        _breaks.Push([]);
        _continues.Push([]);

        node.Initializer?.Accept(this);

        var loopStart = Code.Count;

        node.Condition.Accept(this);

        var exitJump = EmitJump(OpCode.JMP_FALSE);

        EmitOpCode(OpCode.POP);

        node.Body.Accept(this);

        foreach (var @continue in _continues.Peek())
            PatchJump(@continue);

        if (node.Increment is not null)
        {
            node.Increment.Accept(this);
            EmitOpCode(OpCode.POP);
        }

        EmitLoop(OpCode.JMP, loopStart);

        PatchJump(exitJump);

        EmitOpCode(OpCode.POP);

        foreach (var @break in _breaks.Peek())
            PatchJump(@break);

        _continues.Pop();
        _breaks.Pop();
        SymbolRoot.Pop();
    }

    public void Visit(WhileStatement node)
    {
        _breaks.Push([]);
        _continues.Push([]);

        var loopStart = Code.Count;

        node.Condition.Accept(this);

        var exitJump = EmitJump(OpCode.JMP_FALSE);

        EmitOpCode(OpCode.POP);

        node.Body.Accept(this);

        foreach (var @continue in _continues.Peek())
            PatchJump(@continue);

        EmitLoop(OpCode.JMP, loopStart);

        PatchJump(exitJump);

        EmitOpCode(OpCode.POP);

        foreach (var @break in _breaks.Peek())
            PatchJump(@break);

        _continues.Pop();
        _breaks.Pop();
    }

    public void Visit(BreakStatement node) =>
        _breaks.Peek().Add(EmitJump(OpCode.JMP));

    public void Visit(ContinueStatement node) =>
        _continues.Peek().Add(EmitJump(OpCode.JMP));

    public void Visit(BoolLiteralExpression node)
    {
        EmitOpCode(OpCode.BOOL_CONST);
        EmitByte(node.Value ? BoolValue.TRUE : BoolValue.FALSE);
    }

    public void Visit(AssignmentExpression node)
    {
        var identifier = node.Target.As<IdentifierExpression>();
        var assignment = AssignmentOperation.Resolve(node.Operator.Type);
        var offset = Symbols.Offset(identifier.Symbol);

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
            EmitToStringConversion(node.Value.Token, valueType, operation);
            EmitOpCode(operation.OpCode);
        }

        EmitOpCode(OpCode.SET_LOCAL);
        EmitInt32(offset);
    }
}