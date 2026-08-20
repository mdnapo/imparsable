using System.Buffers.Binary;
using System.Text;
using Imparsable.Tool.Calculator.Execution;
using Imparsable.Tool.Calculator.Syntax;
using Imparsable.Virtualization;

namespace Imparsable.Tool.Calculator;

public class Compiler(SyntaxTree tree) : Compiler<OpCode>, ISyntaxVisitor
{
    private static readonly ReadOnlyMemory<byte> FalseBytes = BitConverter.GetBytes((double)0);
    private static readonly ReadOnlyMemory<byte> TrueBytes = BitConverter.GetBytes((double)1);
    private static readonly NumericLiteralExpression ZeroValue = new() { Token = default, Value = 0 };

    private readonly System.Collections.Generic.Stack<List<int>> _elseJumps = [];

    public static Chunk Execute(SyntaxTree tree)
    {
        var compiler = new Compiler(tree);
        foreach (var node in tree.Roots)
            node.Accept(compiler);

        return new Chunk(compiler.Code.ToArray(), compiler.Constants.ToArray());
    }

    public void Visit(BinaryExpression node)
    {
        node.LeftOperand.Accept(this);
        node.RightOperand.Accept(this);

        var text = tree.Source.GetText(node.Operator.Offset, node.Operator.Length);

        var op = node.Operator.Type switch
        {
            Token.PLUS => OpCode.ADD,
            Token.MINUS => OpCode.SUB,
            Token.STAR => OpCode.MUL,
            Token.SLASH => OpCode.DIV,
            Token.LOWER_THAN => OpCode.LOWER_THAN,
            Token.LOWER_EQUAL => OpCode.LOWER_EQUAL,
            Token.GREATER_THAN => OpCode.GREATER_THAN,
            Token.GREATER_EQUAL => OpCode.GREATER_EQUAL,
            Token.EQUAL_EQUAL => OpCode.EQUAL,
            Token.BANG_EQUAL => OpCode.NOT_EQUAL,
            Token.DOT => OpCode.CONCAT,
            _ => throw new InvalidOperationException($"Unsupported operator '{text}' for binary expression.")
        };

        EmitOpCode(op);
    }

    public void Visit(ConstStatement node)
    {
        node.Initializer.Accept(this);
        EmitOpCode(OpCode.SET_LOCAL);
        var offset = tree.SymbolTable.Offset(node.Symbol);
        EmitInt32(offset);
        // EmitOpCode(OpCode.POP);
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
        var offset = tree.SymbolTable.Offset(node.Symbol);
        EmitInt32(offset);
    }

    public void Visit(NumericLiteralExpression node)
    {
        var buffer = AcquireBuffer(sizeof(double));
        var span = buffer.AsSpan()[..sizeof(double)];

        BinaryPrimitives.WriteDoubleLittleEndian(span, node.Value);
        var index = AddConstant(span);
        EmitOpCode(OpCode.NUM_CONST);
        EmitInt32(index);

        ReleaseBuffer(buffer);
    }

    public void Visit(PrintStatement node)
    {
        node.Expression.Accept(this);
        EmitOpCode(OpCode.PRINT);
    }

    public void Visit(StringLiteralExpression node)
    {
        // TODO: Does aligning make sense in the constant pool?
        var @string = node.Value.Trim('\'', '"');
        var length = @string.Length;
        var size = Align(sizeof(int) + length);
        var buffer = AcquireBuffer(size);
        var span = buffer.AsSpan()[..size];
        BinaryPrimitives.WriteInt32LittleEndian(span[..sizeof(int)], length);
        Encoding.UTF8.GetBytes(@string, span[sizeof(int)..]);

        var index = AddConstant(span);
        EmitOpCode(OpCode.STRING_CONST);
        EmitInt32(index);

        ReleaseBuffer(buffer);

        return;

        static int Align(int size) => (size + sizeof(double) - 1) & ~(sizeof(double) - 1);
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
        var offset = tree.SymbolTable.Offset(node.Symbol);
        EmitInt32(offset);
        // EmitOpCode(OpCode.POP);
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
        foreach (var statement in node.Body)
            statement.Accept(this);
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
        if (node.Initializer is not null)
        {
            node.Initializer.Accept(this);
            EmitOpCode(OpCode.POP);
        }

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
        var index = AddConstant(node.Value ? TrueBytes.Span : FalseBytes.Span);
        EmitOpCode(OpCode.NUM_CONST);
        EmitInt32(index);
    }

    public void Visit(AssignmentExpression node)
    {
        if (node.Target is IdentifierExpression identifier)
        {
            node.Value.Accept(this);
            var offset = tree.SymbolTable.Offset(identifier.Symbol);
            EmitOpCode(OpCode.SET_LOCAL);
            EmitInt32(offset);
        }

        throw new InvalidOperationException($"Assignment target of type '{node.GetType()}' is not supported.");
    }
}